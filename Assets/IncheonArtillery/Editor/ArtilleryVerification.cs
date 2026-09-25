using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Incheon.Artillery;
using Object = UnityEngine.Object;

// Runs the real flight/collision code in an isolated physics scene; never rewrites authored scenes or Timeline.
public static class ArtilleryVerification
{
    static readonly MethodInfo Step = typeof(ArtilleryShell).GetMethod("Step", BindingFlags.Instance | BindingFlags.NonPublic);
    static readonly List<string> Results = new List<string>();
    static void Check(bool condition, string message)
    { if (!condition) throw new Exception(message); Results.Add("PASS " + message); }

    [MenuItem("Tools/Incheon Artillery/3 Verify Pool Collision And Effects %&#t")]
    public static void Verify()
    {
        if (EditorApplication.isPlaying) return;
        var original = SceneManager.GetActiveScene();
        var scene = SceneManager.CreateScene("Artillery_Verification_Temporary", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        SceneManager.SetActiveScene(scene); Results.Clear();
        try
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(ArtilleryAssetBuilder.Root + "/Prefabs/Artillery_Barrage.prefab");
            Check(prefab != null, "Barrage prefab exists");
            var root = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
            var rig = root.GetComponent<ArtilleryBarrage>(); rig.automaticFire = false; rig.poolSize = 4;
            rig.snapTargetToGround = false; rig.scatterRadius = 0; rig.useWaterPlane = false; rig.collisionMask = 0;
            rig.launchPoint.position = new Vector3(0,10,-10); rig.flightTime = .5f;rig.arcHeight=4;
            Check(Vector3.Distance(ArtilleryShell.ArcPosition(Vector3.zero,Vector3.forward*10,4,.5f),new Vector3(0,4,5))<.001f,"Arc midpoint and height");
            var target=new Vector3(0,1,6);
            for (int i=0;i<4;i++) Check(rig.FireAt(target),"Pool slot "+i+" launches");
            Check(!rig.FireAt(target)&&rig.PoolCount==4&&rig.ActiveCount==4&&rig.DroppedCount==1,"Capacity stays bounded when full");
            Advance(rig,1f);
            Check(rig.ImpactCount==4&&rig.ActiveCount==4,"Every shot impacts once and remains for effect lifetime");
            Advance(rig,4f);
            Check(rig.ActiveCount==0,"All impact effects return to pool");
            int[] ids=rig.GetComponentsInChildren<ArtilleryShell>(true).Select(s=>s.GetInstanceID()).ToArray();
            for(int i=0;i<80;i++) {Check(rig.FireAt(target),"Repeat shot "+i);Advance(rig,5f);}
            Check(ids.SequenceEqual(rig.GetComponentsInChildren<ArtilleryShell>(true).Select(s=>s.GetInstanceID())),"80 repeat shots reused identical instances");
            Check(rig.ImpactCount==84,"Repeat impacts have no duplicate callback");
            Check(rig.GetComponentsInChildren<TrailRenderer>(true).All(t=>t.positionCount==0),"Returned trails are cleared");
            Check(rig.GetComponentsInChildren<ParticleSystem>(true).All(p=>p.particleCount==0),"Returned particles are cleared");

            var ground=GameObject.CreatePrimitive(PrimitiveType.Cube);ground.layer=3;ground.transform.position=Vector3.zero;ground.transform.localScale=new Vector3(20,1,20);
            Physics.SyncTransforms(); scene.GetPhysicsScene().Simulate(.02f);
            rig.collisionMask=1<<3;rig.launchPoint.position=new Vector3(0,10,0);rig.flightTime=.05f;rig.arcHeight=0;
            Vector3 impact=Vector3.zero;rig.onImpact.AddListener(p=>impact=p);
            rig.FireAt(new Vector3(0,-5,0));Advance(rig,.08f);
            Check(Mathf.Abs(impact.y-.5f)<.04f,"Fast shell hits collider before target instead of tunnelling");
            rig.StopAllShells();
            ground.transform.position=new Vector3(0,-6,0);Physics.SyncTransforms();scene.GetPhysicsScene().Simulate(.02f);
            rig.useWaterPlane=true;rig.waterLevel=0;
            rig.FireAt(new Vector3(0,-6,0));Advance(rig,.08f);
            Check(rig.WaterImpactCount==1&&Mathf.Abs(impact.y)<.01f,"Water plane wins before sea floor collision");
            rig.StopAllShells();rig.useWaterPlane=false;rig.collisionMask=0;rig.flightTime=.5f;
            rig.FireAt(target);root.SetActive(false);
            Check(rig.ActiveCount==0,"Disabling barrage clears all in-flight shells");root.SetActive(true);
            Check(rig.FireAt(target),"Disabled barrage can be reused after enabling");rig.StopAllShells();
            Check(rig.GetComponentsInChildren<Renderer>(true).All(r=>r.sharedMaterials.All(m=>m!=null&&!ShaderUtil.ShaderHasError(m.shader))),"All model and effect shaders compile");
            Results.Add("NOTE deterministic editor simulation; live Play check is separate.");
            CaptureAssets(scene);
        }
        catch(Exception e) {Results.Add("FAIL "+e);Debug.LogException(e);}
        finally
        {
            Directory.CreateDirectory("Temp");File.WriteAllLines("Temp/artillery-verification.txt",Results);
            EditorSceneManager.CloseScene(scene,true);SceneManager.SetActiveScene(original);
            Debug.Log("Artillery verification finished: Temp/artillery-verification.txt");
        }
    }

    static void Advance(ArtilleryBarrage rig,float duration)
    {
        var shells=rig.GetComponentsInChildren<ArtilleryShell>(true);
        for(float time=0;time<duration;time+=.02f)
            foreach(var shell in shells) if(shell.IsBusy) Step.Invoke(shell,new object[]{.02f});
    }

    static void CaptureAssets(Scene scene)
    {
        var origin = new Vector3(10000, 0, 0);
        foreach(var root in scene.GetRootGameObjects()) root.SetActive(false);
        var camera=new GameObject("Preview_Camera").AddComponent<Camera>();camera.enabled=false;
        camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.055f,.07f,.09f);
        camera.fieldOfView=38;camera.nearClipPlane=.03f;camera.farClipPlane=100;
        camera.GetUniversalAdditionalCameraData().renderPostProcessing=false;
        var key=new GameObject("Preview_Key").AddComponent<Light>();key.type=LightType.Directional;key.intensity=2.2f;key.transform.rotation=Quaternion.Euler(35,-35,0);
        RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Flat;RenderSettings.ambientLight=Color.gray;RenderSettings.fog=false;
        var model=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(ArtilleryAssetBuilder.Root+"/Prefabs/Shell_Model.prefab"),scene);
        model.transform.rotation=Quaternion.Euler(0,0,-25);
        model.transform.position=origin;
        camera.transform.position=origin+new Vector3(1.1f,.6f,1.8f);camera.transform.LookAt(origin);
        Render(camera,"Artillery_Model");model.SetActive(false);
        var fx=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(ArtilleryAssetBuilder.Root+"/Prefabs/Shell_Projectile.prefab"),scene);
        var shell=fx.GetComponent<ArtilleryShell>();shell.model.SetActive(false);shell.impactRoot.rotation=Quaternion.identity;
        fx.transform.position=origin;
        camera.transform.position=origin+new Vector3(9,6,-11);camera.transform.LookAt(origin+new Vector3(0,2,0));camera.fieldOfView=48;
        foreach(var ps in shell.groundEffects) {ps.Play(false);ps.Simulate(.4f,false,true,true);}
        Render(camera,"Artillery_Ground_Impact");
        foreach(var ps in shell.groundEffects)ps.Stop(false,ParticleSystemStopBehavior.StopEmittingAndClear);
        foreach(var ps in shell.waterEffects){ps.Play(false);ps.Simulate(.35f,false,true,true);}
        Render(camera,"Artillery_Water_Impact");
    }

    static void Render(Camera camera,string name)
    {
        var rt=new RenderTexture(1200,800,24,RenderTextureFormat.ARGB32);var previous=RenderTexture.active;
        Texture2D image=null;
        try {rt.Create();camera.targetTexture=rt;camera.Render();RenderTexture.active=rt;
            image=new Texture2D(1200,800,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1200,800),0,0);image.Apply();
            Directory.CreateDirectory("Docs/Previews");File.WriteAllBytes("Docs/Previews/"+name+".png",image.EncodeToPNG());}
        finally {RenderTexture.active=previous;camera.targetTexture=null;rt.Release();Object.DestroyImmediate(rt);if(image!=null)Object.DestroyImmediate(image);}
    }
}
