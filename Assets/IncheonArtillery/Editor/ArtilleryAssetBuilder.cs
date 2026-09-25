using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Incheon.Artillery;
using Object = UnityEngine.Object;

public static class ArtilleryAssetBuilder
{
    public const string Root = "Assets/IncheonArtillery";

    [MenuItem("Tools/Incheon Artillery/1 Create Reusable Shell Assets %&#g")]
    public static void Build()
    {
        if (EditorApplication.isPlaying) return;
        if (AssetDatabase.LoadAssetAtPath<GameObject>(Root + "/Prefabs/Artillery_Barrage.prefab") != null)
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(Root + "/Demo/Artillery_Practice.unity") == null) BuildDemoOnly();
            else Debug.Log("Artillery assets already exist. Edit the prefabs directly.");
            return;
        }
        var original = SceneManager.GetActiveScene();
        var preview = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        SceneManager.SetActiveScene(preview);
        try
        {
            foreach (var folder in new[] { "/Materials", "/Meshes", "/Prefabs", "/Demo" }) Directory.CreateDirectory(Root + folder);
            AssetDatabase.Refresh();
            foreach (var name in new[] { "Smoke", "Spark" })
            {
                var importer = (TextureImporter)AssetImporter.GetAtPath(Root + "/Textures/" + name + ".png");
                importer.alphaIsTransparency = true; importer.mipmapEnabled = true; importer.wrapMode = TextureWrapMode.Clamp;
                importer.maxTextureSize = 512; importer.SaveAndReimport();
            }
            var body = Lit("Shell_Olive_Steel", new Color(.23f, .28f, .19f), .68f, .36f);
            var brass = Lit("Shell_Copper_Band", new Color(.65f, .36f, .13f), .8f, .47f);
            var fuse = Lit("Shell_Dark_Tip", new Color(.11f, .12f, .13f), .7f, .45f);
            var smoke = ParticleMaterial("Dust_And_Smoke", "Smoke", false);
            var flash = ParticleMaterial("Flash_And_Sparks", "Spark", true);
            var mist = ParticleMaterial("Water_Spray", "Smoke", false);
            var trailMaterial = ParticleMaterial("Flight_Trace", "Spark", true);

            var model = new GameObject("Shell_Model");
            SceneManager.MoveGameObjectToScene(model, preview);
            MeshPart(model.transform, "Body", new[] {
                new Vector2(-.55f, 0), new Vector2(-.55f, .14f), new Vector2(-.52f, .16f),
                new Vector2(.19f, .16f), new Vector2(.31f, .143f), new Vector2(.42f, .10f), new Vector2(.515f, .051f), new Vector2(.515f, 0) }, body);
            MeshPart(model.transform, "Copper_Band", new[] { new Vector2(-.46f,.163f), new Vector2(-.44f,.174f), new Vector2(-.34f,.174f), new Vector2(-.32f,.163f) }, brass);
            MeshPart(model.transform, "Tip", new[] {new Vector2(.50f,.055f), new Vector2(.55f,.044f), new Vector2(.575f,.02f), new Vector2(.58f,0) }, fuse);
            var modelPrefab = PrefabUtility.SaveAsPrefabAsset(model, Root + "/Prefabs/Shell_Model.prefab");
            Object.DestroyImmediate(model);

            var projectile = new GameObject("Shell_Projectile");
            SceneManager.MoveGameObjectToScene(projectile, preview);
            var shell = projectile.AddComponent<ArtilleryShell>();
            shell.model = (GameObject)PrefabUtility.InstantiatePrefab(modelPrefab, projectile.transform);
            var trace = new GameObject("Flight_Trace"); trace.transform.SetParent(projectile.transform, false);
            trace.transform.localPosition = new Vector3(0, 0, -.55f);
            var trail = trace.AddComponent<TrailRenderer>();
            trail.sharedMaterial = trailMaterial; trail.time = .22f; trail.minVertexDistance = .15f;
            trail.startWidth = .07f; trail.endWidth = .01f; trail.numCapVertices = 2; trail.emitting = false;
            trail.shadowCastingMode = ShadowCastingMode.Off; trail.receiveShadows = false;
            trail.colorGradient = Fade(new Color(1.4f, .65f, .18f, .7f)); shell.flightTrail = trail;
            var impact = new GameObject("Impact_Effects"); impact.transform.SetParent(projectile.transform, false); shell.impactRoot = impact.transform;
            shell.groundEffects = new[] {
                Effect(impact.transform, "Ground_Flash", flash, 4, .24f, 3.5f, 2.3f, new Color(3.5f,1.1f,.18f,1), 75, 0),
                Effect(impact.transform, "Ground_Sparks", flash, 16, .75f, 8f, .16f, new Color(2.3f,.9f,.2f,1), 65, 1.2f, true),
                Effect(impact.transform, "Ground_Dust", smoke, 12, 2.1f, 3.5f, 1.8f, new Color(.43f,.32f,.21f,.7f), 76, .15f),
                Effect(impact.transform, "Ground_Smoke", smoke, 9, 3.1f, 2.4f, 2.5f, new Color(.18f,.17f,.16f,.65f), 18, -.08f) };
            shell.waterEffects = new[] {
                Effect(impact.transform, "Water_Splash", mist, 18, 1.2f, 9f, .4f, new Color(.74f,.88f,.95f,.85f), 32, 1.5f, true),
                Effect(impact.transform, "Water_Surface_Spray", mist, 15, 1.6f, 4f, 1.1f, new Color(.67f,.81f,.88f,.65f), 82, .3f),
                Effect(impact.transform, "Water_Mist", mist, 9, 2.5f, 2f, 2.4f, new Color(.62f,.77f,.84f,.5f), 24, -.02f) };
            var projectilePrefab = PrefabUtility.SaveAsPrefabAsset(projectile, Root + "/Prefabs/Shell_Projectile.prefab");
            Object.DestroyImmediate(projectile);

            var rig = new GameObject("Artillery_Barrage"); SceneManager.MoveGameObjectToScene(rig, preview);
            var barrage = rig.AddComponent<ArtilleryBarrage>(); barrage.shellPrefab = projectilePrefab.GetComponent<ArtilleryShell>();
            var origin = new GameObject("LaunchPoint"); origin.transform.SetParent(rig.transform, false); origin.transform.localPosition = new Vector3(0, 18, -28);
            var target = new GameObject("ImpactTarget"); target.transform.SetParent(rig.transform, false); target.transform.localPosition = new Vector3(0, 1, 12);
            barrage.launchPoint = origin.transform; barrage.impactTarget = target.transform;
            var barragePrefab = PrefabUtility.SaveAsPrefabAsset(rig, Root + "/Prefabs/Artillery_Barrage.prefab");
            Object.DestroyImmediate(rig);
            CreateDemo(barragePrefab, modelPrefab);
            // Save only newly created assets. Do not flush user-authored dynamic font or Timeline assets.
            foreach (var guid in AssetDatabase.FindAssets("", new[] { Root }))
            {
                var asset = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(guid));
                if (asset != null) AssetDatabase.SaveAssetIfDirty(asset);
            }
            Debug.Log("Reusable artillery prefabs and a separate demo scene created. Current scene unchanged.");
        }
        catch (Exception e) { Directory.CreateDirectory("Temp"); File.WriteAllText("Temp/artillery-build-error.txt", e.ToString()); Debug.LogException(e); }
        finally { EditorSceneManager.CloseScene(preview, true); SceneManager.SetActiveScene(original); }
    }

    static Material Lit(string name, Color color, float metallic, float smoothness)
    {
        var m = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = name, enableInstancing = true };
        m.SetColor("_BaseColor", color); m.SetFloat("_Metallic", metallic); m.SetFloat("_Smoothness", smoothness);
        AssetDatabase.CreateAsset(m, Root + "/Materials/" + name + ".mat"); return m;
    }

    static Material ParticleMaterial(string name, string texture, bool additive)
    {
        var m = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit")) { name = name };
        m.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>(Root + "/Textures/" + texture + ".png"));
        m.SetColor("_BaseColor", Color.white); m.SetFloat("_Surface", 1); m.SetFloat("_Blend", additive ? 2 : 0);
        m.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha); m.SetFloat("_DstBlend", additive ? (float)BlendMode.One : (float)BlendMode.OneMinusSrcAlpha);
        m.SetFloat("_ZWrite", 0); m.SetFloat("_Cull", 0); m.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        m.SetOverrideTag("RenderType", "Transparent"); m.renderQueue = 3000;
        AssetDatabase.CreateAsset(m, Root + "/Materials/" + name + ".mat"); return m;
    }

    static Gradient Fade(Color color)
    {
        var g = new Gradient();
        g.SetKeys(new[] { new GradientColorKey(color,0), new GradientColorKey(color,1) },
            new[] {new GradientAlphaKey(color.a,0),new GradientAlphaKey(color.a*.75f,.3f),new GradientAlphaKey(0,1)});
        return g;
    }

    static ParticleSystem Effect(Transform parent, string name, Material material, short count, float life,
        float speed, float size, Color color, float angle, float gravity, bool stretch = false)
    {
        var go = new GameObject(name); go.transform.SetParent(parent, false); go.transform.localRotation = Quaternion.Euler(-90,0,0);
        var ps = go.AddComponent<ParticleSystem>(); ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = ps.main; main.loop = false; main.playOnAwake = false; main.duration = .1f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(life*.7f,life); main.startSpeed = new ParticleSystem.MinMaxCurve(speed*.6f,speed);
        main.startSize = new ParticleSystem.MinMaxCurve(size*.65f,size); main.startRotation = new ParticleSystem.MinMaxCurve(0,Mathf.PI*2);
        main.startColor = Color.white; main.gravityModifier = gravity; main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 48; main.cullingMode = ParticleSystemCullingMode.AlwaysSimulate;
        var emission = ps.emission; emission.rateOverTime = 0; emission.SetBursts(new[] {new ParticleSystem.Burst(0,count)});
        var shape = ps.shape; shape.shapeType = ParticleSystemShapeType.Cone; shape.angle = angle; shape.radius = .25f;
        var colors = ps.colorOverLifetime; colors.enabled = true; colors.color = new ParticleSystem.MinMaxGradient(Fade(color));
        var sizes = ps.sizeOverLifetime; sizes.enabled = true; sizes.size = new ParticleSystem.MinMaxCurve(1,
            new AnimationCurve(new Keyframe(0,.35f), new Keyframe(.35f,1), new Keyframe(1,1.8f)));
        var renderer = ps.GetComponent<ParticleSystemRenderer>(); renderer.sharedMaterial = material;
        renderer.shadowCastingMode = ShadowCastingMode.Off; renderer.receiveShadows = false;
        if (stretch) { renderer.renderMode = ParticleSystemRenderMode.Stretch; renderer.lengthScale = 2; renderer.velocityScale = .08f; }
        return ps;
    }

    static void MeshPart(Transform parent, string name, Vector2[] profile, Material material)
    {
        const int sides = 24;
        var vertices = new List<Vector3>(); var uv = new List<Vector2>(); var triangles = new List<int>();
        for (int row=0; row<profile.Length; row++) for (int side=0; side<=sides; side++)
        {
            float a = side/(float)sides*Mathf.PI*2;
            vertices.Add(new Vector3(Mathf.Cos(a)*profile[row].y, Mathf.Sin(a)*profile[row].y, profile[row].x));
            uv.Add(new Vector2(side/(float)sides,row/(float)(profile.Length-1)));
        }
        for(int row=0;row<profile.Length-1;row++) for(int side=0;side<sides;side++)
        {
            int a=row*(sides+1)+side,b=a+1,c=a+sides+1,d=c+1;
            triangles.Add(a);triangles.Add(b);triangles.Add(c); triangles.Add(c);triangles.Add(b);triangles.Add(d);
        }
        var mesh=new Mesh {name="Shell_"+name}; mesh.SetVertices(vertices); mesh.SetUVs(0,uv);mesh.SetTriangles(triangles,0);
        mesh.RecalculateNormals();mesh.RecalculateBounds();mesh.RecalculateTangents();
        AssetDatabase.CreateAsset(mesh,Root+"/Meshes/"+mesh.name+".asset");
        var go=new GameObject(name);go.transform.SetParent(parent,false);
        go.AddComponent<MeshFilter>().sharedMesh=mesh;go.AddComponent<MeshRenderer>().sharedMaterial=material;
    }

    static void CreateDemo(GameObject barragePrefab, GameObject modelPrefab)
    {
        var scene = SceneManager.GetActiveScene();
        {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Cube); ground.name="Demo_Sand_Ground";ground.layer=3;
            ground.transform.position=new Vector3(-9,-.5f,7);ground.transform.localScale=new Vector3(20,2,40);
            ground.GetComponent<Renderer>().sharedMaterial=Lit("Demo_Sand",new Color(.34f,.28f,.2f),0,.1f);
            var water=GameObject.CreatePrimitive(PrimitiveType.Plane);water.name="Demo_Water_Y0";water.layer=4;
            water.transform.position=new Vector3(12,0,7);water.transform.localScale=new Vector3(2,1,4);
            Object.DestroyImmediate(water.GetComponent<Collider>());
            var existingWater=AssetDatabase.LoadAssetAtPath<Material>("Assets/IncheonEnvironment/Materials/Coastal_Water.mat");
            water.GetComponent<Renderer>().sharedMaterial=existingWater;
            var left=(GameObject)PrefabUtility.InstantiatePrefab(barragePrefab,scene); left.name="Ground_Barrage";
            left.transform.position=new Vector3(-9,0,0);left.GetComponent<ArtilleryBarrage>().scatterRadius=3;
            var right=(GameObject)PrefabUtility.InstantiatePrefab(barragePrefab,scene);right.name="Water_Barrage";right.transform.position=new Vector3(12,0,0);
            var rb=right.GetComponent<ArtilleryBarrage>();rb.impactTarget.localPosition=new Vector3(0,0,12);rb.startDelay=1.6f;rb.randomSeed=26;
            var display=(GameObject)PrefabUtility.InstantiatePrefab(modelPrefab,scene);display.name="Shell_Model_Display";
            display.transform.position=new Vector3(-8,3,-10);display.transform.rotation=Quaternion.Euler(-20,40,0);display.transform.localScale=Vector3.one*3;
            var camera=new GameObject("Demo_Camera").AddComponent<Camera>();camera.transform.position=new Vector3(38,29,-37);camera.transform.LookAt(new Vector3(1,6,4));camera.fieldOfView=52;
            camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.11f,.15f,.20f);
            var extra=camera.GetUniversalAdditionalCameraData();extra.requiresColorTexture=true;extra.requiresDepthTexture=true;
            camera.gameObject.AddComponent<AudioListener>();
            var light=new GameObject("Demo_Light").AddComponent<Light>();light.type=LightType.Directional;light.intensity=1.6f;light.transform.rotation=Quaternion.Euler(48,-28,0);light.shadows=LightShadows.Soft;
            RenderSettings.ambientMode=AmbientMode.Flat;RenderSettings.ambientLight=new Color(.55f,.6f,.68f);RenderSettings.fog=false;
            EditorSceneManager.SaveScene(scene,Root+"/Demo/Artillery_Practice.unity");
        }
    }

    [MenuItem("Tools/Incheon Artillery/2 Create Missing Demo Scene")]
    public static void BuildDemoOnly()
    {
        if (EditorApplication.isPlaying || AssetDatabase.LoadAssetAtPath<SceneAsset>(Root + "/Demo/Artillery_Practice.unity") != null) return;
        var previous = SceneManager.GetActiveScene();
        var demo = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        SceneManager.SetActiveScene(demo);
        try { CreateDemo(AssetDatabase.LoadAssetAtPath<GameObject>(Root + "/Prefabs/Artillery_Barrage.prefab"),
            AssetDatabase.LoadAssetAtPath<GameObject>(Root + "/Prefabs/Shell_Model.prefab")); }
        finally { EditorSceneManager.CloseScene(demo,true);SceneManager.SetActiveScene(previous); }
    }
}
