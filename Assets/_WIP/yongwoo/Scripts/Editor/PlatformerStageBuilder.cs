using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class PlatformerStageBuilder
{
    private const string ScenePath = "Assets/_WIP/yongwoo/Scenes/RefundRun_Platformer.unity";
    private const string RootName = "[Generated] Platformer Map";
    private const string MaterialFolder = "Assets/_WIP/yongwoo/Materials";
    private const string CityBackgroundTexture = "Assets/_WIP/yongwoo/Materials/kthan6v7.jpg";
    private const string CityBackgroundMaterial = "Assets/_WIP/yongwoo/Materials/city background.mat";
    private const string YellowCarPrefab = "Assets/_WIP/yongwoo/ExportAssets/Palmov Island/Low Poly Atmospheric Locations Pack/Prefabs/Vehicles/car SUV large yellow.prefab";
    private const string BrownCarPrefab = "Assets/_WIP/yongwoo/ExportAssets/Palmov Island/Low Poly Atmospheric Locations Pack/Prefabs/Vehicles/sedan car brown.prefab";
    private const string PlayerPrefab = "Assets/_WIP/yongwoo/ExportAssets/Materials/Mini Simple Characters Demo/Prefabs/mini simple demo_05.prefab";
    private const string Tree2Prefab = "Assets/_WIP/yongwoo/ExportAssets/Palmov Island/Low Poly Atmospheric Locations Pack/Prefabs/Trees/tree 2.prefab";
    private const string TreeTrunkPrefab = "Assets/_WIP/yongwoo/ExportAssets/Palmov Island/Low Poly Atmospheric Locations Pack/Prefabs/Trees/Tree trunks/tree trunk.prefab";
    private const string LampPrefab = "Assets/_WIP/yongwoo/ExportAssets/Palmov Island/Low Poly Atmospheric Locations Pack/Prefabs/Environment/lamppost.prefab";
    private const string ChestPrefab = "Assets/_WIP/yongwoo/ExportAssets/Palmov Island/Low Poly Atmospheric Locations Pack/Prefabs/Environment/chest.prefab";
    private const float RoadTopY = 0.5f;
    private const float SurfacePaintY = RoadTopY + 0.002f;
    private const float SurfacePaintHeight = 0.004f;
    private const float PlayerContactSkin = 0.004f;
    private const float PlayerCapsuleHeight = 1.52945f;
    private const float PlayerCapsuleCenterY = 0.73f;
    private const float PlayerSpawnY = RoadTopY + PlayerContactSkin + (PlayerCapsuleHeight * 0.5f) - PlayerCapsuleCenterY;

    [MenuItem("Refund Run/Build RefundRun Platformer Scene")]
    public static void Build()
    {
        var scene = System.IO.File.Exists(ScenePath)
            ? EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single)
            : EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        ClearOpenScene();

        var material = AssetDatabase.LoadAssetAtPath<Material>("Assets/_WIP/yongwoo/ExportAssets/Palmov Island/Low Poly Atmospheric Locations Pack/Materials/mat main.mat");
        var roadMaterial = EnsureMaterial("road asphalt.mat", new Color(0.08f, 0.085f, 0.09f, 1f));
        var sidewalkMaterial = EnsureMaterial("sidewalk concrete.mat", new Color(0.42f, 0.42f, 0.38f, 1f));
        var laneMaterial = EnsureMaterial("lane paint.mat", new Color(0.95f, 0.86f, 0.32f, 1f));
        var curbMaterial = EnsureMaterial("curb paint.mat", new Color(0.82f, 0.82f, 0.78f, 1f));
        var safeMaterial = EnsureMaterial("safe zone paint.mat", new Color(0.25f, 0.45f, 0.33f, 1f));
        var root = new GameObject(RootName).transform;

        EnsureTag("Ground");
        EnsureTag("Obstacle");

        CreatePlatform(root, "Road Segment Start", new Vector3(-24f, 0f, 0f), new Vector3(20f, 1f, 3.2f), roadMaterial);
        CreatePlatform(root, "Road Segment Traffic", new Vector3(-3f, 0f, 0f), new Vector3(18f, 1f, 3.2f), roadMaterial);
        CreatePlatform(root, "Road Segment Puzzle Approach", new Vector3(25f, 0f, 0f), new Vector3(24f, 1f, 3.2f), roadMaterial);
        CreatePlatform(root, "Cross Traffic Road", new Vector3(29f, 0f, 0f), new Vector3(4.6f, 1f, 10.5f), roadMaterial);
        RemoveCollider(CreatePlatform(root, "Start Safe Zone", new Vector3(-32f, SurfacePaintY, 0f), new Vector3(5f, SurfacePaintHeight, 3.15f), safeMaterial));
        RemoveCollider(CreatePlatform(root, "Crossing Wait Zone", new Vector3(23.5f, SurfacePaintY, 0f), new Vector3(3.5f, SurfacePaintHeight, 3.15f), safeMaterial));
        RemoveCollider(CreatePlatform(root, "Quiz Safe Zone", new Vector3(38.5f, SurfacePaintY, 0f), new Vector3(5.5f, SurfacePaintHeight, 3.15f), safeMaterial));
        CreatePlatform(root, "Upper Sidewalk Start", new Vector3(-24f, 0.02f, 2.25f), new Vector3(20f, 0.45f, 0.65f), sidewalkMaterial);
        CreatePlatform(root, "Upper Sidewalk Traffic", new Vector3(-3f, 0.02f, 2.25f), new Vector3(18f, 0.45f, 0.65f), sidewalkMaterial);
        CreatePlatform(root, "Upper Sidewalk Approach", new Vector3(25f, 0.02f, 2.25f), new Vector3(24f, 0.45f, 0.65f), sidewalkMaterial);
        CreatePlatform(root, "Lower Sidewalk Start", new Vector3(-24f, 0.02f, -2.25f), new Vector3(20f, 0.45f, 0.65f), sidewalkMaterial);
        CreatePlatform(root, "Lower Sidewalk Traffic", new Vector3(-3f, 0.02f, -2.25f), new Vector3(18f, 0.45f, 0.65f), sidewalkMaterial);
        CreatePlatform(root, "Lower Sidewalk Approach", new Vector3(25f, 0.02f, -2.25f), new Vector3(24f, 0.45f, 0.65f), sidewalkMaterial);
        RemoveCollider(CreatePlatform(root, "Upper Curb Start", new Vector3(-24f, 0.58f, 1.65f), new Vector3(20f, 0.08f, 0.12f), curbMaterial));
        RemoveCollider(CreatePlatform(root, "Upper Curb Traffic", new Vector3(-3f, 0.58f, 1.65f), new Vector3(18f, 0.08f, 0.12f), curbMaterial));
        RemoveCollider(CreatePlatform(root, "Upper Curb Approach", new Vector3(25f, 0.58f, 1.65f), new Vector3(24f, 0.08f, 0.12f), curbMaterial));
        RemoveCollider(CreatePlatform(root, "Lower Curb Start", new Vector3(-24f, 0.58f, -1.65f), new Vector3(20f, 0.08f, 0.12f), curbMaterial));
        RemoveCollider(CreatePlatform(root, "Lower Curb Traffic", new Vector3(-3f, 0.58f, -1.65f), new Vector3(18f, 0.08f, 0.12f), curbMaterial));
        RemoveCollider(CreatePlatform(root, "Lower Curb Approach", new Vector3(25f, 0.58f, -1.65f), new Vector3(24f, 0.08f, 0.12f), curbMaterial));
        CreateLaneMarkers(root, laneMaterial);
        CreateCrossRoadMarkers(root, laneMaterial);
        CreateCrossStonePlatform(root, material);

        CreateStreetProps(root);

        var player = ConfigurePlayer();
        CreateRoadHazards(root, player.transform);
        CreateDepthTrafficHazards(root, player.transform);
        var quizObject = InstantiatePrefab(ChestPrefab, root, new Vector3(40f, 0.25f, 0f), Quaternion.Euler(0f, -90f, 0f), Vector3.one, "Scale Puzzle Chest");
        ConfigureQuizObject(quizObject, player);
        ConfigureCamera(player.transform);
        ConfigureLighting();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, ScenePath);
        EnsureBuildScene(ScenePath);
        EnsureBuildScene("Assets/_WIP/yongwoo/Scenes/ScalePuzzle.unity");
        AssetDatabase.SaveAssets();
    }

    private static YongwooPlayerController ConfigurePlayer()
    {
        var player = InstantiatePrefab(PlayerPrefab, null, new Vector3(-33f, PlayerSpawnY, 0f), Quaternion.Euler(0f, 90f, 0f), Vector3.one, "Player");
        player.transform.position = new Vector3(-33f, PlayerSpawnY, 0f);
        player.transform.rotation = Quaternion.Euler(0f, 90f, 0f);

        var body = player.GetComponent<Rigidbody>();
        if (body == null)
        {
            body = player.AddComponent<Rigidbody>();
        }

        body.mass = 1f;
        body.useGravity = true;
        body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        var capsule = player.GetComponent<CapsuleCollider>();
        if (capsule == null)
        {
            capsule = player.AddComponent<CapsuleCollider>();
        }

        capsule.height = PlayerCapsuleHeight;
        capsule.radius = 0.3707842f;
        capsule.center = new Vector3(0f, PlayerCapsuleCenterY, 0.02f);

        RemoveScriptByName(player, "NewMoveCS");

        var movement = player.GetComponent<YongwooPlayerController>();
        if (movement == null)
        {
            movement = player.AddComponent<YongwooPlayerController>();
        }
        movement.Speed = 5f;
        movement.JumpForce = 7f;
        movement.lowJumpMultiplier = 0.5f;

        var animator = player.GetComponentInChildren<Animator>();
        var animatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/_WIP/yongwoo/ExportAssets/Materials/Mini Simple Characters Demo/Models/Animations/Mini simple Characters Animation Controller Demo.controller");
        if (animator != null && animator.runtimeAnimatorController == null)
        {
            animator.runtimeAnimatorController = animatorController;
        }

        player.tag = "Player";

        return movement;
    }

    private static void CreateRoadHazards(Transform root, Transform player)
    {
        var carA = InstantiatePrefab(YellowCarPrefab, root, new Vector3(-11f, 0.75f, 0f), Quaternion.Euler(0f, -90f, 0f), Vector3.one * 0.9f, "Traffic Car A");
        ConfigureMovingObstacle(carA, player, new Vector3(-1f, 0f, 0f), 10f, 10f, new Vector3(0f, 0f, 0f), new Vector3(2.5f, 1.4f, 2f), true);

        var carB = InstantiatePrefab(BrownCarPrefab, root, new Vector3(6f, 0.75f, 0f), Quaternion.Euler(0f, -90f, 0f), Vector3.one * 0.95f, "Traffic Car B");
        ConfigureMovingObstacle(carB, player, new Vector3(-1f, 0f, 0f), 12f, 11f, new Vector3(0f, 0f, 0f), new Vector3(2.4f, 1.4f, 2f), true);

        var fakeCar = InstantiatePrefab(YellowCarPrefab, root, new Vector3(14f, 0.75f, 0f), Quaternion.Euler(0f, -90f, 0f), Vector3.one * 0.75f, "Fake Traffic Car");
        ConfigureMovingObstacle(fakeCar, player, new Vector3(-1f, 0f, 0f), 8f, 13f, new Vector3(0f, 0f, 0f), new Vector3(2.2f, 1.2f, 2f), false);

        var trunkA = InstantiatePrefab(TreeTrunkPrefab, root, new Vector3(23f, 0.85f, 0f), Quaternion.Euler(0f, 0f, 90f), new Vector3(1.4f, 1.4f, 1.4f), "Rolling Tree Trunk");
        ConfigureMovingObstacle(trunkA, player, new Vector3(-1f, 0f, 0f), 7f, 12f, new Vector3(0f, 0f, 0f), new Vector3(0.8f, 2.8f, 1.6f), true);

        var fakeTree = InstantiatePrefab(Tree2Prefab, root, new Vector3(30f, 0.5f, 0f), Quaternion.identity, Vector3.one, "Fake Tree Sweep");
        ConfigureMovingObstacle(fakeTree, player, new Vector3(-1f, 0f, 0f), 5f, 9f, new Vector3(0f, 0.7f, 0f), new Vector3(1.1f, 2f, 1.6f), false);
    }

    private static void CreateDepthTrafficHazards(Transform root, Transform player)
    {
        var crossCarA = InstantiatePrefab(YellowCarPrefab, root, new Vector3(29f, 0.75f, 5.6f), Quaternion.Euler(0f, 180f, 0f), Vector3.one * 0.9f, "Cross Traffic Car North");
        ConfigureDepthTrafficObstacle(crossCarA, player, Vector3.back, 11.2f, 8.2f, 13f, 0f);

        var crossCarB = InstantiatePrefab(BrownCarPrefab, root, new Vector3(31f, 0.75f, -5.6f), Quaternion.identity, Vector3.one * 0.88f, "Cross Traffic Car South");
        ConfigureDepthTrafficObstacle(crossCarB, player, Vector3.forward, 11.2f, 7.2f, 12f, 0.75f);
    }

    private static void CreateStreetProps(Transform root)
    {
        for (var i = 0; i < 8; i++)
        {
            float x = -30f + i * 9f;
            InstantiatePrefab(LampPrefab, root, new Vector3(x, 0.05f, 3.15f), Quaternion.identity, Vector3.one, $"Upper Lamp {i + 1}");
        }
    }

    private static void CreateLaneMarkers(Transform root, Material material)
    {
        for (var i = 0; i < 16; i++)
        {
            float x = -30f + i * 4f;
            var marker = CreatePlatform(root, $"Lane Marker {i + 1}", new Vector3(x, SurfacePaintY, 0f), new Vector3(1.6f, SurfacePaintHeight, 0.12f), material);
            var collider = marker.GetComponent<Collider>();
            if (collider != null)
            {
                Object.DestroyImmediate(collider);
            }
        }
    }

    private static void CreateCrossRoadMarkers(Transform root, Material material)
    {
        for (var i = 0; i < 7; i++)
        {
            float z = -4.2f + i * 1.4f;
            var marker = CreatePlatform(root, $"Cross Road Center Marker {i + 1}", new Vector3(30f, SurfacePaintY, z), new Vector3(0.12f, SurfacePaintHeight, 0.75f), material);
            RemoveCollider(marker);
        }

        RemoveCollider(CreatePlatform(root, "Cross Road Warning Before", new Vector3(25.7f, SurfacePaintY, 0f), new Vector3(0.18f, SurfacePaintHeight, 3.15f), material));
        RemoveCollider(CreatePlatform(root, "Cross Road Warning After", new Vector3(32.4f, SurfacePaintY, 0f), new Vector3(0.18f, SurfacePaintHeight, 3.15f), material));
    }

    private static void CreateCrossStonePlatform(Transform root, Material material)
    {
        var bridge = new GameObject("Timing Cross Stone");
        bridge.transform.SetParent(root);
        bridge.transform.position = new Vector3(12f, 0.35f, 0f);
        var body = bridge.AddComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;
        bridge.AddComponent<OscillatingPlatform>().localOffset = new Vector3(0f, 0.9f, 0f);

        CreateLocalPlatform(bridge.transform, "Cross Center", Vector3.zero, new Vector3(1.7f, 0.35f, 1.7f), material);
        CreateLocalPlatform(bridge.transform, "Cross Left", new Vector3(-1.7f, 0f, 0f), new Vector3(1.7f, 0.35f, 1.1f), material);
        CreateLocalPlatform(bridge.transform, "Cross Right", new Vector3(1.7f, 0f, 0f), new Vector3(1.7f, 0.35f, 1.1f), material);
        CreateLocalPlatform(bridge.transform, "Cross Up", new Vector3(0f, 0f, 1.25f), new Vector3(1.1f, 0.35f, 1.4f), material);
        CreateLocalPlatform(bridge.transform, "Cross Down", new Vector3(0f, 0f, -1.25f), new Vector3(1.1f, 0.35f, 1.4f), material);
    }

    private static void ConfigureQuizObject(GameObject quizObject, YongwooPlayerController player)
    {
        var trigger = quizObject.GetComponent<BoxCollider>();
        if (trigger == null)
        {
            trigger = quizObject.AddComponent<BoxCollider>();
        }

        trigger.isTrigger = true;
        trigger.center = new Vector3(0f, 1f, 0f);
        trigger.size = new Vector3(3f, 2.2f, 3f);

        var quizTrigger = quizObject.GetComponent<ScalePuzzleTrigger>();
        if (quizTrigger == null)
        {
            quizTrigger = quizObject.AddComponent<ScalePuzzleTrigger>();
        }

        SerializedSet(quizTrigger, "playerMovement", player);
    }

    private static void ConfigureCamera(Transform target)
    {
        var camera = Camera.main;
        if (camera == null)
        {
            var cameraGo = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            camera = cameraGo.GetComponent<Camera>();
            cameraGo.tag = "MainCamera";
        }

        camera.orthographic = true;
        camera.orthographicSize = 7.5f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.58f, 0.72f, 0.82f, 1f);
        camera.transform.position = new Vector3(target.position.x, target.position.y + 3.75f, -14f);
        camera.transform.rotation = Quaternion.Euler(15f, 0f, 0f);

        RemoveScriptByName(camera.gameObject, "CameraFollow");

        var follow = camera.GetComponent<YongwooRoadCameraFollow>();
        if (follow == null)
        {
            follow = camera.gameObject.AddComponent<YongwooRoadCameraFollow>();
        }

        follow.target = target;
        follow.offsetY = 3.75f;
        follow.fixedZ = -14f;
        follow.followSpeed = 6f;
        follow.xAngle = 15f;

        CreateCameraBackground(camera);
    }

    private static void CreateCameraBackground(Camera camera)
    {
        var oldBackground = camera.transform.Find("City Background");
        if (oldBackground != null)
        {
            Object.DestroyImmediate(oldBackground.gameObject);
        }

        var material = EnsureBackgroundMaterial();
        if (material == null)
        {
            return;
        }

        var background = GameObject.CreatePrimitive(PrimitiveType.Quad);
        background.name = "City Background";
        background.transform.SetParent(camera.transform);
        background.transform.localPosition = new Vector3(0f, 0f, 32f);
        background.transform.localRotation = Quaternion.identity;
        background.transform.localScale = new Vector3(28.8f, 16.2f, 1f);

        var collider = background.GetComponent<Collider>();
        if (collider != null)
        {
            Object.DestroyImmediate(collider);
        }

        var renderer = background.GetComponent<MeshRenderer>();
        renderer.sharedMaterial = material;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;
    }

    private static Material EnsureBackgroundMaterial()
    {
        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(CityBackgroundTexture);
        if (texture == null)
        {
            return null;
        }

        var material = AssetDatabase.LoadAssetAtPath<Material>(CityBackgroundMaterial);
        if (material == null)
        {
            var shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null)
            {
                shader = Shader.Find("Unlit/Texture");
            }

            material = new Material(shader);
            AssetDatabase.CreateAsset(material, CityBackgroundMaterial);
        }

        if (material.HasProperty("_BaseMap"))
        {
            material.SetTexture("_BaseMap", texture);
        }

        if (material.HasProperty("_MainTex"))
        {
            material.SetTexture("_MainTex", texture);
        }

        return material;
    }

    private static void ConfigureLighting()
    {
        var lightGo = new GameObject("Directional Light", typeof(Light));
        lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        var light = lightGo.GetComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.2f;
    }

    private static GameObject CreatePlatform(Transform parent, string name, Vector3 position, Vector3 scale, Material material)
    {
        var platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
        platform.name = name;
        platform.transform.SetParent(parent);
        platform.transform.position = position;
        platform.transform.localScale = scale;

        if (material != null)
        {
            platform.GetComponent<MeshRenderer>().sharedMaterial = material;
        }

        platform.tag = "Ground";
        return platform;
    }

    private static GameObject RemoveCollider(GameObject gameObject)
    {
        var collider = gameObject.GetComponent<Collider>();
        if (collider != null)
        {
            Object.DestroyImmediate(collider);
        }

        return gameObject;
    }

    private static GameObject CreateLocalPlatform(Transform parent, string name, Vector3 localPosition, Vector3 scale, Material material)
    {
        var platform = CreatePlatform(parent, name, parent.TransformPoint(localPosition), scale, material);
        platform.transform.localPosition = localPosition;
        platform.transform.localRotation = Quaternion.identity;
        return platform;
    }

    private static void ConfigureMovingObstacle(GameObject obstacle, Transform player, Vector3 direction, float speed, float triggerDistance, Vector3 colliderCenter, Vector3 colliderSize, bool lethal)
    {
        if (obstacle == null)
        {
            return;
        }

        SetTagRecursive(obstacle, lethal ? "Obstacle" : "Untagged");

        foreach (var childCollider in obstacle.GetComponentsInChildren<Collider>(true))
        {
            childCollider.enabled = false;
        }

        var collider = obstacle.GetComponent<BoxCollider>();
        if (collider == null)
        {
            collider = obstacle.AddComponent<BoxCollider>();
        }

        collider.center = colliderCenter;
        collider.size = colliderSize;
        collider.isTrigger = false;
        collider.enabled = lethal;

        var body = obstacle.GetComponent<Rigidbody>();
        if (body == null)
        {
            body = obstacle.AddComponent<Rigidbody>();
        }

        body.isKinematic = true;
        body.useGravity = false;

        RemoveScriptByName(obstacle, "MovingTrap");

        var trap = obstacle.GetComponent<YongwooMovingTrap>();
        if (trap == null)
        {
            trap = obstacle.AddComponent<YongwooMovingTrap>();
        }

        trap.player = player;
        trap.moveDirection = direction.normalized;
        trap.moveSpeed = speed;
        trap.triggerDistance = triggerDistance;
    }

    private static void ConfigureDepthTrafficObstacle(GameObject obstacle, Transform player, Vector3 direction, float travelDistance, float speed, float triggerDistance, float startDelay)
    {
        if (obstacle == null)
        {
            return;
        }

        SetTagRecursive(obstacle, "Obstacle");

        foreach (var childCollider in obstacle.GetComponentsInChildren<Collider>(true))
        {
            childCollider.enabled = false;
        }

        var collider = obstacle.GetComponent<BoxCollider>();
        if (collider == null)
        {
            collider = obstacle.AddComponent<BoxCollider>();
        }

        collider.center = Vector3.zero;
        collider.size = new Vector3(2.2f, 1.4f, 2.8f);
        collider.isTrigger = true;
        collider.enabled = true;

        var body = obstacle.GetComponent<Rigidbody>();
        if (body == null)
        {
            body = obstacle.AddComponent<Rigidbody>();
        }

        body.isKinematic = true;
        body.useGravity = false;

        RemoveScriptByName(obstacle, "MovingTrap");

        var traffic = obstacle.GetComponent<YongwooDepthTrafficObstacle>();
        if (traffic == null)
        {
            traffic = obstacle.AddComponent<YongwooDepthTrafficObstacle>();
        }

        traffic.player = player;
        traffic.moveDirection = direction.normalized;
        traffic.travelDistance = travelDistance;
        traffic.moveSpeed = speed;
        traffic.triggerDistance = triggerDistance;
        traffic.startDelay = startDelay;
        traffic.loopDelay = 1.1f;
    }

    private static void SetTagRecursive(GameObject root, string tag)
    {
        root.tag = tag;
        foreach (Transform child in root.transform)
        {
            SetTagRecursive(child.gameObject, tag);
        }
    }

    private static void RemoveScriptByName(GameObject target, string scriptName)
    {
        foreach (var script in target.GetComponents<MonoBehaviour>())
        {
            if (script != null && script.GetType().Name == scriptName)
            {
                Object.DestroyImmediate(script);
            }
        }
    }

    private static GameObject InstantiatePrefab(string path, Transform parent, Vector3 position, Quaternion rotation, Vector3 scale, string name)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null)
        {
            var fallback = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fallback.name = name;
            if (parent != null)
            {
                fallback.transform.SetParent(parent);
            }
            fallback.transform.position = position;
            fallback.transform.rotation = rotation;
            fallback.transform.localScale = scale;
            return fallback;
        }

        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.name = name;
        if (parent != null)
        {
            instance.transform.SetParent(parent);
        }
        instance.transform.position = position;
        instance.transform.rotation = rotation;
        instance.transform.localScale = scale;
        return instance;
    }

    private static Material EnsureMaterial(string fileName, Color color)
    {
        Directory.CreateDirectory(MaterialFolder);
        string path = $"{MaterialFolder}/{fileName}";
        var material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material != null)
        {
            material.color = color;
            return material;
        }

        var shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
        {
            shader = Shader.Find("Standard");
        }

        material = new Material(shader)
        {
            color = color
        };
        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    private static void SerializedSet(Object target, string fieldName, object value)
    {
        var serialized = new SerializedObject(target);
        var property = serialized.FindProperty(fieldName);

        if (property == null)
        {
            return;
        }

        if (value is Object objectValue)
        {
            property.objectReferenceValue = objectValue;
        }
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void ClearOpenScene()
    {
        var scene = EditorSceneManager.GetActiveScene();
        foreach (var root in scene.GetRootGameObjects())
        {
            Object.DestroyImmediate(root);
        }
    }

    private static void EnsureBuildScene(string path)
    {
        var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        if (scenes.Exists(scene => scene.path == path))
        {
            return;
        }

        scenes.Add(new EditorBuildSettingsScene(path, true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }

    private static void EnsureTag(string tag)
    {
        var tagManager = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0];
        var serialized = new SerializedObject(tagManager);
        var tags = serialized.FindProperty("tags");

        for (var i = 0; i < tags.arraySize; i++)
        {
            if (tags.GetArrayElementAtIndex(i).stringValue == tag)
            {
                return;
            }
        }

        tags.InsertArrayElementAtIndex(tags.arraySize);
        tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tag;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }
}
