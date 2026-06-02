using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class PlatformerStageBuilder
{
    private const string ScenePath = "Assets/_WIP/yongwoo/Scenes/RefundRun_Platformer.unity";
    private const string RootName = "[Generated] Platformer Map";

    [MenuItem("Refund Run/Build RefundRun Platformer Scene")]
    public static void Build()
    {
        var scene = System.IO.File.Exists(ScenePath)
            ? EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single)
            : EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        ClearOpenScene();

        var material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Palmov Island/Low Poly Atmospheric Locations Pack/Materials/mat main.mat");
        var root = new GameObject(RootName).transform;

        EnsureTag("Ground");

        CreatePlatform(root, "Ground", new Vector3(0f, -0.55f, 0f), new Vector3(34f, 1f, 5f), material);
        CreatePlatform(root, "Step A", new Vector3(-8f, 1.1f, 0f), new Vector3(5f, 0.45f, 4f), material);
        CreatePlatform(root, "Step B", new Vector3(-1.5f, 2.5f, 0f), new Vector3(5f, 0.45f, 4f), material);
        CreatePlatform(root, "Step C", new Vector3(6f, 3.9f, 0f), new Vector3(5f, 0.45f, 4f), material);
        CreatePlatform(root, "Quiz Ledge", new Vector3(13f, 1.35f, 0f), new Vector3(5.5f, 0.45f, 4f), material);

        InstantiatePrefab("Assets/Palmov Island/Low Poly Atmospheric Locations Pack/Prefabs/Location with environment/downtown with environment.prefab", root, new Vector3(0f, -1f, 8f), Quaternion.identity, new Vector3(0.7f, 0.7f, 0.7f), "Downtown Backdrop");
        InstantiatePrefab("Assets/Palmov Island/Low Poly Atmospheric Locations Pack/Prefabs/Trees/tree.prefab", root, new Vector3(-13f, 0f, -1.3f), Quaternion.identity, Vector3.one, "Tree Marker");
        InstantiatePrefab("Assets/Palmov Island/Low Poly Atmospheric Locations Pack/Prefabs/Environment/lamppost.prefab", root, new Vector3(3f, 0f, -1.2f), Quaternion.identity, Vector3.one, "Lamp Marker");
        var quizObject = InstantiatePrefab("Assets/Palmov Island/Low Poly Atmospheric Locations Pack/Prefabs/Environment/chest.prefab", root, new Vector3(13f, 2f, 0f), Quaternion.identity, Vector3.one, "Quiz Chest");

        var player = ConfigurePlayer();
        ConfigureQuizObject(quizObject, player);
        ConfigureCamera(player.transform);
        ConfigureLighting();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, ScenePath);
        EnsureBuildScene(ScenePath);
        EnsureBuildScene("Assets/_WIP/yongwoo/Scenes/ScalePuzzle.unity");
        AssetDatabase.SaveAssets();
    }

    private static NewMoveCS ConfigurePlayer()
    {
        var player = InstantiatePrefab("Assets/Materials/Mini Simple Characters Demo/Prefabs/mini simple demo_01.prefab", null, new Vector3(-14f, 0.75f, 0f), Quaternion.identity, Vector3.one, "Player");
        player.transform.position = new Vector3(-14f, 0.75f, 0f);
        player.transform.rotation = Quaternion.identity;

        var body = player.GetComponent<Rigidbody>();
        if (body == null)
        {
            body = player.AddComponent<Rigidbody>();
        }

        body.mass = 1f;
        body.useGravity = true;

        var capsule = player.GetComponent<CapsuleCollider>();
        if (capsule == null)
        {
            capsule = player.AddComponent<CapsuleCollider>();
        }

        capsule.height = 1.8f;
        capsule.radius = 0.38f;
        capsule.center = new Vector3(0f, 0.9f, 0f);

        var movement = player.GetComponent<NewMoveCS>();
        if (movement == null)
        {
            movement = player.AddComponent<NewMoveCS>();
        }

        var animator = player.GetComponentInChildren<Animator>();
        var animatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Materials/Mini Simple Characters Demo/Models/Animations/Mini simple Characters Animation Controller Demo.controller");
        if (animator != null && animator.runtimeAnimatorController == null)
        {
            animator.runtimeAnimatorController = animatorController;
        }

        player.tag = "Player";

        return movement;
    }

    private static void ConfigureQuizObject(GameObject quizObject, NewMoveCS player)
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
        camera.orthographicSize = 6f;
        camera.transform.position = new Vector3(target.position.x, target.position.y + 2.5f, -12f);
        camera.transform.rotation = Quaternion.identity;

        var follow = camera.GetComponent<CameraFollow>();
        if (follow == null)
        {
            follow = camera.gameObject.AddComponent<CameraFollow>();
        }

        follow.target = target;
        follow.offsetY = 2.5f;
        follow.offsetZ = -12f;
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
