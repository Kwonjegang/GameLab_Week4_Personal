using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

// Sequence01 전용 점검/설정 도구. 카메라 키와 배의 이동 키는 수정하지 않습니다.
public static class ShipPassengerSetup
{
    [InitializeOnLoadMethod]
    static void Register()
    {
        // Normal editor use does not run this batch verification automatically.
        if (Application.isBatchMode && SessionState.GetBool("ShipProbe", false))
            EditorApplication.update += ProbeUpdate;
    }
    [MenuItem("Tools/Incheon Ship/Audit Passengers")]
    public static void Audit()
    {
        var b = new StringBuilder();
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        b.AppendLine($"scene={scene.path} dirty={scene.isDirty} playing={EditorApplication.isPlaying}");
        var all = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Transform>(true)).ToArray();
        var ship = all.Single(t => t.name == "ship-cargo-c");
        var actors = ship.Cast<Transform>().Where(t => t.name.StartsWith("Unitychan")).ToArray();
        b.AppendLine($"SHIP world={ship.position:F4} rotation={ship.eulerAngles:F4} scale={ship.lossyScale:F4}");
        foreach (var t in actors)
        {
            var a=t.GetComponent<Animator>(); var cc=t.GetComponent<CharacterController>(); var m=t.GetComponent<Movement>();
            b.AppendLine($"{t.name} local={t.localPosition:F4} world={t.position:F4} scale={t.localScale:F4} move={m?.isMoveable} CC={cc?.enabled} animator={a?.enabled} avatar={a?.avatar?.name} valid={a?.avatar?.isValid} human={a?.isHuman} rootMotion={a?.applyRootMotion} controller={a?.runtimeAnimatorController?.name}");
            if(a!=null && a.isHuman) foreach(var bone in new[]{HumanBodyBones.Hips,HumanBodyBones.Head,HumanBodyBones.LeftFoot,HumanBodyBones.RightFoot})
                b.AppendLine($"  {bone}: root-relative={t.InverseTransformPoint(a.GetBoneTransform(bone).position):F4}");
        }
        foreach(var d in all.SelectMany(t=>t.GetComponents<PlayableDirector>()))
            b.AppendLine($"DIRECTOR {d.name} time={d.time:F4} state={d.state} graph={d.playableGraph.IsValid()}");
        // Sample source motions on a disposable actor in an isolated preview scene.
        var preview=EditorSceneManager.NewPreviewScene();
        try
        {
            var clone=Object.Instantiate(actors[1].gameObject);
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(clone,preview);
            clone.transform.SetPositionAndRotation(Vector3.zero,Quaternion.identity); clone.transform.localScale=Vector3.one;
            foreach(var m in clone.GetComponents<MonoBehaviour>()) Object.DestroyImmediate(m);
            Object.DestroyImmediate(clone.GetComponent<CharacterController>());
            var a=clone.GetComponent<Animator>(); a.runtimeAnimatorController=null; a.applyRootMotion=false;
            foreach(var path in new[]{"Assets/CameraDrill/Character/Animations/UnitychanRFN_STD_FN_Crouch_Idle.fbx","Assets/CameraDrill/Character/Animations/UnitychanRFN_STD_FN_Sitdown_Chair_All.fbx","Assets/CameraDrill/Character/Animations/UnitychanRFN_STD_FN_Creeping_All.fbx"})
            foreach(var clip in AssetDatabase.LoadAllAssetsAtPath(path).OfType<AnimationClip>().Where(c=>!c.name.StartsWith("__")))
            {
                b.AppendLine($"CLIP {clip.name} length={clip.length:F4} human={clip.isHumanMotion}");
                foreach(float t in new[]{0f,.5f,1f})
                {
                    clip.SampleAnimation(clone,t*clip.length);
                    b.AppendLine($"  {t:F1}: root={clone.transform.localPosition:F4} hips={a.GetBoneTransform(HumanBodyBones.Hips).position:F4} head={a.GetBoneTransform(HumanBodyBones.Head).position:F4} foot={a.GetBoneTransform(HumanBodyBones.LeftFoot).position:F4}");
                }
            }
        }
        finally {EditorSceneManager.ClosePreviewScene(preview);}
        File.WriteAllText("UserSettings/ship-passenger-audit.txt",b.ToString());
    }
    public static void AuditBatch()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Incheon_Practice.unity");
        Audit();
    }
    public static void ProbeBatch()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Incheon_Practice.unity");
        SessionState.SetBool("ShipProbe",true);
        EditorApplication.EnterPlaymode();
    }
    public static void SetupBatch()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Incheon_Practice.unity");
        Setup();
    }
    // One-time migration entry point; kept off the menu after scene setup.
    public static void Setup()
    {
        if(EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play mode before setup.");
        var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if(scene.path!="Assets/Scenes/Incheon_Practice.unity" || scene.isDirty)
            throw new InvalidOperationException("Open and save Incheon_Practice before setup.");
        var director=GameObject.Find("Sequence01_Ship").GetComponent<PlayableDirector>();
        var timeline=(TimelineAsset)director.playableAsset;
        if(timeline.GetOutputTracks().Any(t=>t.name.StartsWith("Companion ")))
            throw new InvalidOperationException("Passenger setup is already present. Edit the existing Timeline tracks.");
        const string timelinePath="Assets/Timeline/Sequence01.playable";
        string backup="UserSettings/ShipPassengerBackups/"+DateTime.Now.ToString("yyyyMMdd-HHmmss");
        Directory.CreateDirectory(backup);
        File.Copy(scene.path,backup+"/Incheon_Practice.unity");
        File.Copy(timelinePath,backup+"/Sequence01.playable");
        var ship=GameObject.Find("ship-cargo-c").transform;
        foreach(Transform actor in ship)
        {
            if(!actor.name.StartsWith("Unitychan"))continue;
            var animator=actor.GetComponent<Animator>();
            Undo.RecordObject(animator,"Keep passenger on ship");
            animator.applyRootMotion=false;
            animator.cullingMode=AnimatorCullingMode.AlwaysAnimate;
            PrefabUtility.RecordPrefabInstancePropertyModifications(animator);
            var cc=actor.GetComponent<CharacterController>();
            Undo.RecordObject(cc,"Disable passenger collision movement");cc.enabled=false;
            var movement=actor.GetComponent<Movement>();
            Undo.RecordObject(movement,"Disable passenger input");movement.isMoveable=false;movement.enabled=false;
        }
        var crouch=AssetDatabase.LoadAllAssetsAtPath("Assets/CameraDrill/Character/Animations/UnitychanRFN_STD_FN_Crouch_Idle.fbx").OfType<AnimationClip>().Single(c=>!c.name.StartsWith("__"));
        var stand=AssetDatabase.LoadAllAssetsAtPath("Assets/CameraDrill/Character/Animations/UnitychanRFN_STD_FN_Sitdown_Chair_All.fbx").OfType<AnimationClip>().Single(c=>c.name.EndsWith("_End"));
        AddStandTrack(timeline,director,ship.Find("UnitychanRFN_STD (1)").GetComponent<Animator>(),"Companion Right - Stand",20.0,crouch,stand);
        AddStandTrack(timeline,director,ship.Find("UnitychanRFN_STD (2)").GetComponent<Animator>(),"Companion Left - Stand",21.85,crouch,stand);
        EditorUtility.SetDirty(timeline);EditorUtility.SetDirty(director);
        AssetDatabase.SaveAssetIfDirty(timeline);
        EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
        File.WriteAllText("UserSettings/ship-passenger-setup.txt","Done. Backup: "+backup);
    }
    static void AddStandTrack(TimelineAsset timeline,PlayableDirector director,Animator actor,string name,double start,AnimationClip crouch,AnimationClip stand)
    {
        if(timeline.GetOutputTracks().Any(t=>t.name==name))throw new InvalidOperationException("Passenger track already exists: "+name);
        var track=timeline.CreateTrack<AnimationTrack>(null,name);
        track.trackOffset=TrackOffset.ApplySceneOffsets;
        var idle=track.CreateClip(crouch);
        idle.displayName="Crouch - wait";idle.start=0;idle.duration=start+.35;
        ((AnimationPlayableAsset)idle.asset).applyFootIK=false;
        var rise=track.CreateClip(stand);
        rise.displayName="Stand up";rise.start=start;rise.duration=1.6;
        rise.timeScale=stand.length/rise.duration;
        idle.blendOutDuration=.35;
        rise.blendInDuration=.35;
        ((AnimationPlayableAsset)rise.asset).applyFootIK=false;
        // AnimationTrack creates clips with Hold extrapolation by default.
        director.SetGenericBinding(track,actor);
    }
    static readonly double[] Times = {0,15.1,20,28.1,34.8,35.4,35.9,36.7,37.7,38.5,40.5,42,44.3};
    static int probeIndex;
    static readonly StringBuilder probe = new StringBuilder();
    static void ProbeUpdate()
    {
        if(!SessionState.GetBool("ShipProbe",false)||!EditorApplication.isPlaying||EditorApplication.isCompiling) return;
        try
        {
            var master=GameObject.Find("SequenceManager").GetComponent<PlayableDirector>();
            Time.captureDeltaTime=.05f;
            // Keep the graph playing: pausing it lets the Animator Controller take over.
            // This follows the real automatic Opening -> Sequence01 playback.
            if(master.time<Times[probeIndex])return;
            var ship=GameObject.Find("ship-cargo-c").transform;
            probe.AppendLine($"TIME {master.time:F4} SHIP {ship.position:F4}");
            foreach(Transform t in ship)
            {
                if(!t.name.StartsWith("Unitychan"))continue;
                var a=t.GetComponent<Animator>();
                var head=a.GetBoneTransform(HumanBodyBones.Head);
                probe.AppendLine($"{t.name} local={t.localPosition:F4} world={t.position:F4} headLocal={t.InverseTransformPoint(head.position):F4} state={a.GetCurrentAnimatorStateInfo(0).fullPathHash}");
            }
            File.WriteAllText("UserSettings/ship-passenger-play.txt",probe.ToString());
            probeIndex++;
            if(probeIndex==Times.Length)
            {
                SessionState.SetBool("ShipProbe",false);
                Time.captureDeltaTime=0;
                EditorApplication.ExitPlaymode();
                EditorApplication.delayCall+=()=>EditorApplication.Exit(0);
            }
        }
        catch(Exception e){File.WriteAllText("UserSettings/ship-passenger-error.txt",e.ToString());SessionState.SetBool("ShipProbe",false);EditorApplication.Exit(1);}
    }
}
