using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

// Editor-only: the generated terrain, rocks and water remain ordinary editable scene assets.
public static class IncheonBackgroundBuilder
{
    const string Folder = "Assets/IncheonEnvironment";
    const string Common = "Assets/Samples/Shader Graph/Common";
    const string Samples = "Assets/Samples/Shader Graph/17.0.4/Production Ready Shaders";
    const string RootName = "Coast_Background";
    const float TerrainX = -384f, TerrainZ = -480f, TerrainBase = -16f;
    const float Width = 768f, Depth = 640f, Height = 256f;
    const float BeachHeight = 7.373312f;
    static readonly string[] MarkerNames = { "ShipStartPoint", "ShipStopPoint", "EyeContackPoint", "TeamDeadPoint", "OpeningWakeUpPoint" };

    [MenuItem("Tools/Incheon Background/1 Build From Current Layout")]
    public static void Build()
    {
        try
        {
            var scene = SceneManager.GetActiveScene();
            if (EditorApplication.isPlaying || scene.path != "Assets/Scenes/Incheon_Practice.unity")
                throw new InvalidOperationException("Open Incheon_Practice in Edit mode first.");
            if (GameObject.Find(RootName) != null)
                throw new InvalidOperationException("Background already exists. Edit the terrain and materials directly; build is intentionally one-time.");

            var protectedObjects = scene.GetRootGameObjects().Where(g =>
                MarkerNames.Contains(g.name) || g.name == "Main Camera" || g.name == "CM_Opening" ||
                g.name == "Actors" || g.name.StartsWith("Unitychan")).ToArray();
            var positions = protectedObjects.Select(g => g.transform.position).ToArray();
            var rotations = protectedObjects.Select(g => g.transform.rotation).ToArray();
            var scales = protectedObjects.Select(g => g.transform.localScale).ToArray();
            EditorSceneManager.SaveScene(scene);
            Directory.CreateDirectory("UserSettings/EnvironmentBackups");
            File.Copy(scene.path, "UserSettings/EnvironmentBackups/Incheon_Practice-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".unity", false);
            Directory.CreateDirectory(Folder + "/Materials");
            Directory.CreateDirectory(Folder + "/Terrain");
            AssetDatabase.Refresh();
            ConfigureTexture("Sand_Albedo.jpg", false);
            ConfigureTexture("Coast_Albedo.jpg", false);
            ConfigureTexture("Sand_Normal.exr", true);
            ConfigureTexture("Coast_Normal.exr", true);

            var root = new GameObject(RootName);
            Undo.RegisterCreatedObjectUndo(root, "Create Incheon background");
            var environment = GameObject.Find("Enviroment") ?? GameObject.Find("Environment");
            if (environment != null) root.transform.SetParent(environment.transform, true);
            root.transform.position = Vector3.zero;
            root.transform.rotation = Quaternion.identity;
            root.transform.localScale = Vector3.one;

            var terrain = CreateTerrain(root.transform);
            CreateWater(root.transform);
            CreateRocks(root.transform, terrain);
            SetAtmosphere();
            // Keep the user's blockout in place for comparison or restoration.
            foreach (var name in new[] { "Sea", "Sand", "BeachSlope", "Mountain" })
            {
                var blockout = GameObject.Find(name);
                if (blockout != null)
                {
                    Undo.RecordObject(blockout, "Keep original blockout hidden");
                    blockout.SetActive(false);
                }
            }
            // Markers stay at their original locations. Labels are drawn in the Scene view.
            foreach (var name in MarkerNames)
            {
                var marker = GameObject.Find(name);
                if (marker == null) continue;
                foreach (var renderer in marker.GetComponentsInChildren<Renderer>())
                { Undo.RecordObject(renderer, "Hide marker geometry"); renderer.enabled = false; }
                foreach (var collider in marker.GetComponentsInChildren<Collider>())
                { Undo.RecordObject(collider, "Disable guide collider"); collider.enabled = false; }
            }
            for (int i = 0; i < protectedObjects.Length; i++)
            {
                var tr = protectedObjects[i].transform;
                if (tr.position != positions[i] || tr.rotation != rotations[i] || tr.localScale != scales[i])
                    throw new InvalidOperationException("Protected layout changed: " + tr.name);
            }
            AssetDatabase.SaveAssets();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            FrameCoast();
            Validate();
            Debug.Log("Incheon background built. Original markers, actors and camera transforms preserved.");
        }
        catch (Exception exception)
        {
            Directory.CreateDirectory("Temp");
            File.WriteAllText("Temp/incheon-background-error.txt", exception.ToString());
            Debug.LogException(exception);
        }
    }

    static T Load<T>(string path) where T : Object
    {
        var asset = AssetDatabase.LoadAssetAtPath<T>(path);
        if (asset == null) throw new FileNotFoundException("Required existing asset: " + path);
        return asset;
    }

    static void ConfigureTexture(string name, bool normal)
    {
        string path = Folder + "/Textures/" + name;
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) throw new FileNotFoundException(path);
        importer.textureType = normal ? TextureImporterType.NormalMap : TextureImporterType.Default;
        importer.sRGBTexture = !normal;
        importer.maxTextureSize = 2048;
        importer.mipmapEnabled = true;
        importer.wrapMode = TextureWrapMode.Repeat;
        importer.anisoLevel = 4;
        importer.SaveAndReimport();
    }

    static TerrainLayer Layer(string name, string albedo, string normal, float tile, float smoothness)
    {
        var layer = new TerrainLayer
        {
            name = name, diffuseTexture = Load<Texture2D>(albedo), normalMapTexture = Load<Texture2D>(normal),
            tileSize = new Vector2(tile, tile), normalScale = .65f, metallic = 0, smoothness = smoothness
        };
        AssetDatabase.CreateAsset(layer, Folder + "/Terrain/" + name + ".terrainlayer");
        return layer;
    }

    static Terrain CreateTerrain(Transform parent)
    {
        var sand = Layer("Dry_Sand", Folder + "/Textures/Sand_Albedo.jpg", Folder + "/Textures/Sand_Normal.exr", 5f, .08f);
        var coast = Layer("Wet_Coast", Folder + "/Textures/Coast_Albedo.jpg", Folder + "/Textures/Coast_Normal.exr", 7f, .26f);
        var grass = Object.Instantiate(Load<TerrainLayer>(Common + "/Textures/terrain/ground_grass_fells_mossy/ground_grass_fells_mossy.terrainlayer"));
        grass.name = "Hill_Grass"; grass.tileSize = new Vector2(12, 12); grass.normalScale = .7f;
        AssetDatabase.CreateAsset(grass, Folder + "/Terrain/Hill_Grass.terrainlayer");
        var stone = Object.Instantiate(Load<TerrainLayer>(Common + "/Textures/terrain/stone_ground/stone_ground.terrainlayer"));
        stone.name = "Mountain_Stone"; stone.tileSize = new Vector2(15, 15); stone.normalScale = .85f;
        AssetDatabase.CreateAsset(stone, Folder + "/Terrain/Mountain_Stone.terrainlayer");
        var data = new TerrainData { name = "Incheon_Coast_Terrain", heightmapResolution = 1025, alphamapResolution = 512,
            baseMapResolution = 1024, size = new Vector3(Width, Height, Depth), terrainLayers = new[] { sand, coast, grass, stone } };
        var heights = new float[1025, 1025];
        for (int z = 0; z < 1025; z++)
            for (int x = 0; x < 1025; x++)
                heights[z, x] = (GroundHeight(TerrainX + x / 1024f * Width, TerrainZ + z / 1024f * Depth) - TerrainBase) / Height;
        data.SetHeights(0, 0, heights);
        AssetDatabase.CreateAsset(data, Folder + "/Terrain/Incheon_Coast_Terrain.asset");
        PaintTerrain(data);
        var go = Terrain.CreateTerrainGameObject(data);
        go.name = "Coastal_Terrain";
        go.transform.SetParent(parent, false);
        go.transform.position = new Vector3(TerrainX, TerrainBase, TerrainZ);
        go.layer = 3;
        var terrain = go.GetComponent<Terrain>();
        var material = new Material(Shader.Find("Universal Render Pipeline/Terrain/Lit")) { name = "Coastal_Terrain_Lit" };
        material.EnableKeyword("_TERRAIN_INSTANCED_PERPIXEL_NORMAL");
        AssetDatabase.CreateAsset(material, Folder + "/Materials/Coastal_Terrain_Lit.mat");
        terrain.materialTemplate = material;
        terrain.drawInstanced = true; terrain.heightmapPixelError = 5; terrain.basemapDistance = 600;
        return terrain;
    }

    static void PaintTerrain(TerrainData data)
    {
        var alpha = new float[512, 512, 4];
        for (int z = 0; z < 512; z++)
            for (int x = 0; x < 512; x++)
            {
                float u = x / 511f, v = z / 511f;
                float wx = TerrainX + u * Width, wz = TerrainZ + v * Depth;
                float h = GroundHeight(wx, wz), slope = data.GetSteepness(u, v);
                float noise = Mathf.PerlinNoise(wx * .045f + 50f, wz * .045f + 50f);
                float hills = Smooth(10f, 23f, h) * Smooth(-45f, -95f, wz) + Smooth(19f, 42f, h);
                hills = Mathf.Clamp01(hills);
                float rock = hills * Mathf.Clamp01(Smooth(22f, 47f, slope) * .8f + Smooth(65, 115, h) * .4f + (noise - .5f) * .28f);
                float wet = (1f - hills) * (1f - Smooth(.3f, 3.8f, h)) * .88f;
                float grassy = hills - rock;
                float dry = Mathf.Max(0, 1f - wet - grassy - rock);
                alpha[z, x, 0] = dry; alpha[z, x, 1] = wet; alpha[z, x, 2] = grassy; alpha[z, x, 3] = rock;
            }
        data.SetAlphamaps(0, 0, alpha);
        EditorUtility.SetDirty(data);
    }

    static float Smooth(float a, float b, float value) => Mathf.SmoothStep(0, 1, Mathf.Clamp01((value - a) / (b - a)));
    static float Gaussian(float x, float z, float cx, float cz, float rx, float rz)
        => Mathf.Exp(-((x - cx) * (x - cx) / (rx * rx) + (z - cz) * (z - cz) / (rz * rz)) * 1.5f);

    public static float GroundHeight(float x, float z)
    {
        // The central approach follows the user's original 32-degree shore ramp.
        float corridor = 1f - Smooth(15, 55, Mathf.Abs(x + 6.4f));
        float coastBend = (Mathf.Sin(x * .021f) * 7f + Mathf.Sin(x * .047f + .8f) * 3f) * (1f - corridor);
        float coastZ = z - coastBend;
        float shore = Mathf.Lerp(-6f, BeachHeight, Smooth(45, 19, coastZ));
        float originalSlope = Mathf.Clamp(4.1f - (coastZ - 27.3f) * Mathf.Tan(32.019f * Mathf.Deg2Rad), -6f, BeachHeight);
        shore = Mathf.Lerp(shore, originalSlope, corridor * Smooth(44, 37, coastZ));
        float beachNoise = (Mathf.PerlinNoise(x * .036f + 13, z * .034f + 76) - .5f) * .65f;
        beachNoise *= (1 - corridor) * Smooth(1, 7, shore);
        float ridge = Mathf.Max(100 * Gaussian(x, z, -12, -258, 140, 120),
                      Mathf.Max(82 * Gaussian(x, z, -147, -239, 125, 120), 108 * Gaussian(x, z, 116, -309, 148, 108)));
        ridge = Mathf.Max(ridge, 82 * Gaussian(x, z, -224, -376, 150, 115));
        float crags = Mathf.PerlinNoise(x * .016f + 90, z * .018f + 60) * .4f +
                      Mathf.PerlinNoise(x * .047f + 7, z * .043f + 9) * .18f +
                      Mathf.PerlinNoise(x * .11f + 2, z * .12f + 4) * .06f;
        ridge *= .68f + crags;
        ridge *= Smooth(-62, -133, z);
        float headland = 38 * Gaussian(x, z, -128, -28, 48, 94) + 29 * Gaussian(x, z, 124, -60, 55, 96);
        headland *= Smooth(42, -12, z);
        // Keep the opening and action area level with the user's original sand block.
        float openArea = (1 - Smooth(30, 65, Mathf.Abs(x - 1f))) * Smooth(23, 12, z) * Smooth(-94, -58, z);
        float result = shore + beachNoise + ridge + headland;
        return Mathf.Lerp(result, BeachHeight, openArea);
    }

    static void CreateWater(Transform parent)
    {
        var source = Load<Material>(Samples + "/Environment/Water/WaterLake.mat");
        var water = new Material(source) { name = "Coastal_Water" };
        water.SetColor("_Color", new Color(.20f, .48f, .51f, 0));
        water.SetColor("_DepthColor", new Color(.035f, .115f, .16f, 0));
        water.SetFloat("_OpaqueDepth", 5f);
        water.SetFloat("_RefractionStrength", .008f);
        water.SetVector("_RippleScale", new Vector4(.18f, .14f, .11f, .09f));
        water.SetVector("_RippleSpeed", new Vector4(-.15f, .018f, -.035f, -.14f));
        AssetDatabase.CreateAsset(water, Folder + "/Materials/Coastal_Water.mat");
        var surface = new GameObject("Coastal_Water_Surface");
        surface.transform.SetParent(parent, false);
        surface.transform.position = new Vector3(0, 0, 480);
        surface.transform.localScale = new Vector3(800, 1, 800);
        var temp = GameObject.CreatePrimitive(PrimitiveType.Plane);
        var plane = temp.GetComponent<MeshFilter>().sharedMesh;
        Object.DestroyImmediate(temp);
        surface.AddComponent<MeshFilter>().sharedMesh = plane;
        var renderer = surface.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = water; renderer.shadowCastingMode = ShadowCastingMode.Off;
        renderer.receiveShadows = true;
    }

    static void CreateRocks(Transform parent, Terrain terrain)
    {
        var cluster = new GameObject("Coastal_Rocks"); cluster.transform.SetParent(parent, false);
        var material = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = "Coastal_Rock" };
        material.SetTexture("_BaseMap", Load<Texture2D>(Common + "/Textures/Rock_A_CS.png"));
        material.SetColor("_BaseColor", new Color(.71f, .72f, .70f));
        material.SetTexture("_BumpMap", Load<Texture2D>(Folder + "/Textures/Coast_Normal.exr"));
        material.SetFloat("_BumpScale", .35f); material.SetFloat("_Smoothness", .16f); material.EnableKeyword("_NORMALMAP");
        AssetDatabase.CreateAsset(material, Folder + "/Materials/Coastal_Rock.mat");
        var random = new System.Random(19500915);
        for (int i = 0; i < 36; i++)
        {
            float side = i % 2 == 0 ? -1 : 1;
            float x = side * (40f + (float)random.NextDouble() * 73f);
            float z = 19f - (float)random.NextDouble() * 99f;
            var prefab = Load<GameObject>(Common + "/Meshes/Rock_A_0" + (i % 2 + 1) + ".prefab");
            var rock = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            rock.name = "Shore_Rock_" + (i + 1).ToString("00"); rock.transform.SetParent(cluster.transform, false);
            var renderers = rock.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) throw new InvalidOperationException("Rock mesh missing");
            Bounds bounds = renderers[0].bounds;
            foreach (var r in renderers) bounds.Encapsulate(r.bounds);
            float targetSize = 1.5f + (float)random.NextDouble() * 4.8f;
            float scale = targetSize / Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            rock.transform.localScale = new Vector3(scale * 1.3f, scale * .8f, scale);
            rock.transform.rotation = Quaternion.Euler((float)random.NextDouble() * 16 - 8, (float)random.NextDouble() * 360, 0);
            rock.transform.position = new Vector3(x, terrain.SampleHeight(new Vector3(x, 0, z)) + TerrainBase - targetSize * .14f, z);
            foreach (var r in renderers) r.sharedMaterials = Enumerable.Repeat(material, r.sharedMaterials.Length).ToArray();
        }
    }

    static void SetAtmosphere()
    {
        var sky = new Material(Shader.Find("Skybox/Procedural")) { name = "Coastal_Haze_Sky" };
        sky.SetFloat("_SunSize", .035f); sky.SetFloat("_AtmosphereThickness", 1.05f);
        sky.SetColor("_SkyTint", new Color(.48f, .53f, .58f)); sky.SetColor("_GroundColor", new Color(.43f, .47f, .49f));
        sky.SetFloat("_Exposure", 1.05f);
        AssetDatabase.CreateAsset(sky, Folder + "/Materials/Coastal_Haze_Sky.mat");
        RenderSettings.skybox = sky;
        RenderSettings.ambientMode = AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(.53f, .62f, .68f);
        RenderSettings.ambientEquatorColor = new Color(.39f, .43f, .44f);
        RenderSettings.ambientGroundColor = new Color(.24f, .22f, .19f);
        RenderSettings.fog = true; RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogColor = new Color(.60f, .70f, .74f);
        RenderSettings.fogStartDistance = 170f; RenderSettings.fogEndDistance = 1000f;
        var light = GameObject.Find("Directional Light")?.GetComponent<Light>();
        if (light != null)
        {
            Undo.RecordObject(light, "Coastal daylight");
            light.color = new Color(1f, .94f, .83f); light.intensity = 1.4f;
            light.shadows = LightShadows.Soft; RenderSettings.sun = light;
        }
        DynamicGI.UpdateEnvironment();
    }

    [MenuItem("Tools/Incheon Background/2 View Coast In Scene")]
    public static void FrameCoast()
    {
        var view = SceneView.lastActiveSceneView;
        if (view == null) return;
        var target = new Vector3(-8, 15, -66);
        var eye = new Vector3(170, 118, 140);
        view.LookAt(target, Quaternion.LookRotation(target - eye), 170f, false, true);
        view.Repaint();
    }

    [MenuItem("Tools/Incheon Background/3 Capture Background Previews")]
    public static void CapturePreviews()
    {
        // Defer rendering until outside the menu's OnGUI event.
        EditorApplication.delayCall += () =>
        {
            try
            {
                Directory.CreateDirectory("Docs/Previews");
                Capture("Coast_Overview", new Vector3(155, 96, 132), new Vector3(-14, 23, -90), 58);
                Capture("Beach_Toward_Mountains", new Vector3(-6.4f, 11, 14), new Vector3(-12, 48, -250), 64);
                Capture("Shore_Toward_Sea", new Vector3(17, 11, -24), new Vector3(-6, 0, 76), 64);
                Capture("Water_Close", new Vector3(-6.4f, 2.5f, 40.4f), new Vector3(-6, 0, 58), 64);
                Validate();
                Debug.Log("Background previews saved to Docs/Previews.");
            }
            catch (Exception e) { File.WriteAllText("Temp/incheon-preview-error.txt", e.ToString()); Debug.LogException(e); }
        };
    }

    static void Capture(string name, Vector3 position, Vector3 target, float fov)
    {
        var go = new GameObject("Temporary_Environment_Preview") { hideFlags = HideFlags.HideAndDontSave };
        var camera = go.AddComponent<Camera>();
        camera.enabled = false; camera.transform.position = position; camera.transform.LookAt(target);
        camera.fieldOfView = fov; camera.nearClipPlane = .1f; camera.farClipPlane = 2200;
        var extra = camera.GetUniversalAdditionalCameraData();
        extra.renderPostProcessing = true; extra.requiresDepthTexture = true; extra.requiresColorTexture = true;
        var rt = new RenderTexture(1600, 900, 24, RenderTextureFormat.ARGB32);
        var previous = RenderTexture.active;
        Texture2D image = null;
        try
        {
            rt.Create(); camera.targetTexture = rt;
            camera.Render();
            RenderTexture.active = rt;
            image = new Texture2D(1600, 900, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, 1600, 900), 0, 0); image.Apply();
            File.WriteAllBytes("Docs/Previews/" + name + ".png", image.EncodeToPNG());
        }
        finally
        {
            RenderTexture.active = previous; camera.targetTexture = null;
            rt.Release(); Object.DestroyImmediate(rt); Object.DestroyImmediate(go);
            if (image != null) Object.DestroyImmediate(image);
        }
    }

    [MenuItem("Tools/Incheon Background/4 Validate Background")]
    public static void Validate()
    {
        var root = GameObject.Find(RootName);
        if (root == null) throw new InvalidOperationException("Background root not found");
        var terrain = root.GetComponentInChildren<Terrain>();
        var materials = root.GetComponentsInChildren<Renderer>().SelectMany(r => r.sharedMaterials).Where(m => m != null).Distinct().ToList();
        materials.Add(terrain.materialTemplate);
        var errors = materials.Where(m => m.shader == null || ShaderUtil.ShaderHasError(m.shader)).Select(m => m.name).ToArray();
        var markerInfo = MarkerNames.Select(name =>
        {
            var marker = SceneManager.GetActiveScene().GetRootGameObjects()
                .SelectMany(g => g.GetComponentsInChildren<Transform>(true)).FirstOrDefault(t => t.name == name);
            if (marker == null) return name + " MISSING";
            var tr = marker;
            float ground = terrain.SampleHeight(tr.position) + terrain.transform.position.y;
            return name + " position=" + tr.position.ToString("F3") + " ground=" + ground.ToString("F3") + " waterDepth=" + (-ground).ToString("F3");
        }).ToArray();
        var summary = new ValidationReport { terrainResolution = terrain.terrainData.heightmapResolution,
            terrainLayers = terrain.terrainData.terrainLayers.Length, rockCount = root.transform.Find("Coastal_Rocks").childCount,
            materialShaderErrors = errors, markers = markerInfo,
            waterHasCollider = root.transform.Find("Coastal_Water_Surface").GetComponent<Collider>() != null,
            activeScene = SceneManager.GetActiveScene().path };
        Directory.CreateDirectory("Temp");
        File.WriteAllText("Temp/incheon-background-validation.json", JsonUtility.ToJson(summary, true));
        if (errors.Length > 0) throw new InvalidOperationException("Background shader errors: " + string.Join(", ", errors));
    }

    [Serializable] class ValidationReport
    {
        public string activeScene;
        public int terrainResolution, terrainLayers, rockCount;
        public string[] materialShaderErrors, markers;
        public bool waterHasCollider;
    }

    [InitializeOnLoadMethod]
    static void RegisterMarkerLabels()
    {
        SceneView.duringSceneGui -= DrawMarkerLabels;
        SceneView.duringSceneGui += DrawMarkerLabels;
    }

    static void DrawMarkerLabels(SceneView view)
    {
        if (SceneManager.GetActiveScene().path != "Assets/Scenes/Incheon_Practice.unity") return;
        foreach (var name in MarkerNames)
        {
            var marker = GameObject.Find(name);
            if (marker == null) continue;
            Handles.color = new Color(1, .65f, .12f, .9f);
            Handles.DrawWireDisc(marker.transform.position, Vector3.up, .8f);
            Handles.Label(marker.transform.position + Vector3.up * .7f, name);
        }
    }

    [MenuItem("Tools/Incheon Background/5 Inspect Rendering")]
    static void InspectRendering()
    {
        var terrain = GameObject.Find(RootName).GetComponentInChildren<Terrain>();
        var water = GameObject.Find("Coastal_Water_Surface");
        var report = new System.Text.StringBuilder();
        report.AppendLine("Water mesh: " + water.GetComponent<MeshFilter>().sharedMesh.name);
        report.AppendLine("Water mesh bounds: " + water.GetComponent<MeshFilter>().sharedMesh.bounds);
        report.AppendLine("Water world bounds: " + water.GetComponent<Renderer>().bounds);
        var data = terrain.terrainData;
        foreach (var pos in new[] { new Vector2(-12, -258), new Vector2(-6, 12), new Vector2(-6, 40) })
        {
            int x = Mathf.Clamp(Mathf.RoundToInt((pos.x - TerrainX) / Width * (data.alphamapWidth - 1)), 0, data.alphamapWidth - 1);
            int z = Mathf.Clamp(Mathf.RoundToInt((pos.y - TerrainZ) / Depth * (data.alphamapHeight - 1)), 0, data.alphamapHeight - 1);
            var a = data.GetAlphamaps(x, z, 1, 1);
            report.AppendLine(pos + " alpha: " + string.Join(",", Enumerable.Range(0, 4).Select(i => a[0, 0, i].ToString("F3"))));
        }
        foreach (var layer in data.terrainLayers) report.AppendLine(layer.name + ": " + layer.diffuseTexture.name);
        File.WriteAllText("Temp/incheon-rendering-inspection.txt", report.ToString());
    }

    // Internal one-time construction correction. Kept off the menu to protect later manual terrain edits.
    static void RefineGeneratedBackground()
    {
        if (EditorApplication.isPlaying || SceneManager.GetActiveScene().path != "Assets/Scenes/Incheon_Practice.unity") return;
        var root = GameObject.Find(RootName);
        if (root == null) return;
        var terrain = root.GetComponentInChildren<Terrain>();
        var data = terrain.terrainData;
        var heights = new float[1025, 1025];
        for (int z = 0; z < 1025; z++)
            for (int x = 0; x < 1025; x++)
                heights[z, x] = (GroundHeight(TerrainX + x / 1024f * Width, TerrainZ + z / 1024f * Depth) - TerrainBase) / Height;
        data.SetHeights(0, 0, heights);
        PaintTerrain(data);
        terrain.Flush();
        var water = root.transform.Find("Coastal_Water_Surface");
        var primitive = GameObject.CreatePrimitive(PrimitiveType.Plane);
        water.GetComponent<MeshFilter>().sharedMesh = primitive.GetComponent<MeshFilter>().sharedMesh;
        Object.DestroyImmediate(primitive);
        foreach (Transform rock in root.transform.Find("Coastal_Rocks"))
        {
            var p = rock.position;
            // Preserve each rock's embed depth relative to its bounds.
            var renderer = rock.GetComponentInChildren<Renderer>();
            var bounds = renderer.bounds;
            p.y += terrain.SampleHeight(p) + TerrainBase - bounds.min.y - bounds.size.y * .2f;
            rock.position = p;
        }
        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        InspectRendering();
        FrameCoast();
        CapturePreviews();
    }

    [MenuItem("Tools/Incheon Background/6 Audit Current Connections %&#i")]
    static void AuditCurrentConnections()
    {
        var scene = SceneManager.GetActiveScene();
        var objects = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Transform>(true)).Select(t => t.gameObject).ToArray();
        var report = new System.Text.StringBuilder();
        report.AppendLine("Scene: " + scene.path + "; dirty=" + scene.isDirty + "; playing=" + EditorApplication.isPlaying);
        foreach (var go in objects)
        {
            int missing = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
            if (missing > 0) report.AppendLine("MISSING SCRIPT: " + go.name + " x" + missing);
            foreach (var component in go.GetComponents<Component>().Where(c => c != null))
            {
                using (var serialized = new SerializedObject(component))
                {
                    var property = serialized.GetIterator();
                    while (property.NextVisible(true))
                        if (property.propertyType == SerializedPropertyType.ObjectReference &&
                            property.objectReferenceValue == null && property.objectReferenceInstanceIDValue != 0)
                            report.AppendLine("MISSING REFERENCE: " + go.name + "/" + component.GetType().Name + "/" + property.propertyPath);
                }
            }
        }
        foreach (var director in objects.SelectMany(g => g.GetComponents<UnityEngine.Playables.PlayableDirector>()))
        {
            report.AppendLine("Director: " + director.name + "; asset=" + director.playableAsset?.name + "; duration=" + director.duration + "; playOnAwake=" + director.playOnAwake);
            if (director.playableAsset == null) continue;
            foreach (var output in director.playableAsset.outputs)
            {
                var binding = director.GetGenericBinding(output.sourceObject);
                report.AppendLine("  " + output.streamName + " -> " + (binding != null ? binding.name + " (" + binding.GetType().Name + ")" : "UNBOUND"));
            }
        }
        foreach (var camera in objects.SelectMany(g => g.GetComponents<Camera>()))
        {
            var urp = camera.GetComponent<UniversalAdditionalCameraData>();
            report.AppendLine("Camera: " + camera.name + "; pos=" + camera.transform.position + "; far=" + camera.farClipPlane + "; postFX=" + (urp != null && urp.renderPostProcessing));
            report.AppendLine("  Components: " + string.Join(", ", camera.GetComponents<Component>().Where(c => c != null).Select(c => c.GetType().Name)));
        }
        foreach (var volume in objects.SelectMany(g => g.GetComponents<Volume>()))
            report.AppendLine("Volume: " + volume.name + "; profile=" + volume.sharedProfile?.name + "; weight=" + volume.weight + "; priority=" + volume.priority);
        foreach (var animator in objects.SelectMany(g => g.GetComponents<Animator>()))
            report.AppendLine("Animator: " + animator.name + "; controller=" + animator.runtimeAnimatorController?.name);
        report.AppendLine("Sky: " + RenderSettings.skybox?.name);
        Directory.CreateDirectory("Temp");
        File.WriteAllText("Temp/incheon-current-connections.txt", report.ToString());
        Validate();
        InspectRendering();
        Debug.Log("Current scene connections audited without changing the scene or Timeline.");
    }

    [MenuItem("Tools/Incheon Background/7 Extend Water Horizon %&#w")]
    static void ExtendWaterHorizon()
    {
        var scene = SceneManager.GetActiveScene();
        if (EditorApplication.isPlaying || scene.path != "Assets/Scenes/Incheon_Practice.unity") return;
        var root = GameObject.Find(RootName);
        var surface = root != null ? root.transform.Find("Coastal_Water_Surface") : null;
        if (surface == null) throw new InvalidOperationException("Existing water surface not found.");
        Undo.RecordObject(surface, "Extend existing water horizon");
        var scale = surface.localScale;
        surface.localScale = new Vector3(Mathf.Max(scale.x, 800), scale.y, Mathf.Max(scale.z, 800));
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AuditCurrentConnections();
        CapturePreviews();
    }
}
