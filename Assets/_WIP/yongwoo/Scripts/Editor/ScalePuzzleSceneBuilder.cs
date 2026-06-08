using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class ScalePuzzleSceneBuilder
{
    private const string ScenePath = "Assets/_WIP/yongwoo/Scenes/ScalePuzzle.unity";
    private const string FontSourcePath = "C:/Windows/Fonts/NotoSansKR-VF.ttf";
    private const string FontCopyPath = "Assets/_WIP/yongwoo/ExportAssets/Fonts/NotoSansKR-VF.ttf";
    private static readonly Color Background = new Color(0.10f, 0.12f, 0.15f);
    private static readonly Color Panel = new Color(0.18f, 0.21f, 0.25f, 0.94f);
    private static readonly Color PanelAlt = new Color(0.25f, 0.30f, 0.32f, 0.96f);
    private static readonly Color Paper = new Color(0.88f, 0.86f, 0.76f, 0.98f);
    private static readonly Color Brass = new Color(0.66f, 0.56f, 0.30f, 1f);
    private static readonly Color Ink = new Color(0.07f, 0.08f, 0.09f, 1f);
    private static Font koreanFont;
    private const bool LowEndUi = true;

    [MenuItem("Refund Run/Build Scale Puzzle Scene")]
    public static void BuildScene()
    {
        Directory.CreateDirectory("Assets/Scenes");
        EnsureKoreanFontAsset();

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "ScalePuzzle";

        CreateCamera();
        GameObject canvas = CreateCanvas();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        CreatePanel(canvas.transform, "Background", Vector2.zero, new Vector2(1920f, 1080f), Background, false);
        CreateHeader(canvas.transform);

        GameObject controllerObject = new GameObject("Scale Puzzle Controller");
        ScalePuzzleController controller = controllerObject.AddComponent<ScalePuzzleController>();
        controller.templates = CreateTemplates();

        RectTransform board = CreatePanel(canvas.transform, "Puzzle Board", new Vector2(0f, -15f), new Vector2(1560f, 880f), Panel, true);
        RectTransform problemCard = CreatePanel(board, "Problem Card", new Vector2(0f, 315f), new Vector2(1260f, 150f), Paper, true);
        controller.puzzleText = CreateText(problemCard, "Problem Text", string.Empty, Vector2.zero, new Vector2(1180f, 120f), 34, Ink, TextAnchor.MiddleCenter);

        controller.remainingText = CreateText(board, "Weighing Counter", string.Empty, new Vector2(-590f, 210f), new Vector2(360f, 48f), 28, Color.white, TextAnchor.MiddleLeft);
        controller.answerText = CreateText(board, "Answer Counter", string.Empty, new Vector2(590f, 210f), new Vector2(360f, 48f), 28, Color.white, TextAnchor.MiddleRight);

        RectTransform scaleArea = CreatePanel(board, "Scale Area", new Vector2(0f, 45f), new Vector2(1200f, 370f), new Color(0.13f, 0.16f, 0.19f, 0.65f), true);
        CreateScale(controller, scaleArea);

        controller.feedbackText = CreateText(board, "Feedback", string.Empty, new Vector2(0f, -170f), new Vector2(980f, 48f), 27, Color.white, TextAnchor.MiddleCenter);
        controller.historyText = CreateText(board, "History", string.Empty, new Vector2(0f, -235f), new Vector2(1180f, 92f), 22, new Color(0.84f, 0.88f, 0.9f), TextAnchor.MiddleCenter);

        RectTransform poolPanel = CreatePanel(board, "Weight Pool", new Vector2(-220f, -390f), new Vector2(960f, 190f), PanelAlt, true);
        CreateText(poolPanel, "Pool Label", "추를 끌어서 양쪽 접시에 올리세요", new Vector2(0f, 72f), new Vector2(760f, 32f), 20, new Color(0.82f, 0.86f, 0.88f), TextAnchor.MiddleCenter);
        controller.poolContent = CreateDropContent(controller, poolPanel, "Pool Content", ScalePuzzleController.Zone.Pool, new Vector2(0f, -15f), new Vector2(890f, 120f), true);

        RectTransform actionPanel = CreatePanel(board, "Action Panel", new Vector2(585f, -390f), new Vector2(330f, 190f), new Color(0.21f, 0.25f, 0.28f, 0.96f), true);
        controller.weighButton = CreateButton(actionPanel, "Weigh Button", "저울질", new Vector2(-82f, 45f), new Vector2(142f, 58f), 25, new Color(0.85f, 0.78f, 0.49f));
        controller.decideButton = CreateButton(actionPanel, "Decide Button", "결정", new Vector2(82f, 45f), new Vector2(142f, 58f), 25, new Color(0.54f, 0.78f, 0.54f));
        controller.resetButton = CreateButton(actionPanel, "Reset Button", "초기화", new Vector2(-82f, -35f), new Vector2(142f, 58f), 25, new Color(0.74f, 0.76f, 0.78f));
        controller.newPuzzleButton = CreateButton(actionPanel, "New Button", "새 문제", new Vector2(82f, -35f), new Vector2(142f, 58f), 25, new Color(0.52f, 0.70f, 0.88f));

        controller.dragLayer = CreateRect(canvasRect, "Drag Layer", Vector2.zero, new Vector2(1920f, 1080f));
        controller.weights = CreateWeights(controller, canvas, controller.poolContent);

        CreateEventSystem();
        EditorSceneManager.SaveScene(scene, ScenePath);
        AddSceneToBuildSettings();
        AssetDatabase.Refresh();
    }

    private static ScalePuzzleController.PuzzleTemplate[] CreateTemplates()
    {
        return new[]
        {
            new ScalePuzzleController.PuzzleTemplate { title = "8개의 추 중 하나만 더 가볍습니다.", weightCount = 8, oddWeightKind = ScalePuzzleController.WeightKind.Lighter, maxWeighs = 2 },
            new ScalePuzzleController.PuzzleTemplate { title = "8개의 추 중 하나만 더 무겁습니다.", weightCount = 8, oddWeightKind = ScalePuzzleController.WeightKind.Heavier, maxWeighs = 2 },
            new ScalePuzzleController.PuzzleTemplate { title = "9개의 추 중 하나만 더 가볍습니다.", weightCount = 9, oddWeightKind = ScalePuzzleController.WeightKind.Lighter, maxWeighs = 2 },
            new ScalePuzzleController.PuzzleTemplate { title = "9개의 추 중 하나만 더 무겁습니다.", weightCount = 9, oddWeightKind = ScalePuzzleController.WeightKind.Heavier, maxWeighs = 2 },
            new ScalePuzzleController.PuzzleTemplate { title = "12개의 추 중 하나만 더 가볍습니다.", weightCount = 12, oddWeightKind = ScalePuzzleController.WeightKind.Lighter, maxWeighs = 3 },
            new ScalePuzzleController.PuzzleTemplate { title = "12개의 추 중 하나만 더 무겁습니다.", weightCount = 12, oddWeightKind = ScalePuzzleController.WeightKind.Heavier, maxWeighs = 3 }
        };
    }

    private static void CreateHeader(Transform parent)
    {
        RectTransform header = CreatePanel(parent, "Top Strip", new Vector2(0f, 510f), new Vector2(1920f, 60f), new Color(0.46f, 0.55f, 0.48f, 1f), false);
        CreateText(header, "Stage Label", "환불런 / 저울 퍼즐", new Vector2(-570f, 0f), new Vector2(520f, 42f), 24, Color.white, TextAnchor.MiddleLeft);
        CreateText(header, "Puzzle Number", "무작위 문제", new Vector2(585f, 0f), new Vector2(420f, 42f), 24, Color.white, TextAnchor.MiddleRight);
    }

    private static void CreateScale(ScalePuzzleController controller, RectTransform parent)
    {
        RectTransform stand = CreatePanel(parent, "Scale Stand", new Vector2(0f, -52f), new Vector2(62f, 210f), new Color(0.62f, 0.65f, 0.58f), true);
        CreatePanel(stand, "Stand Highlight", new Vector2(-16f, 0f), new Vector2(10f, 190f), new Color(0.76f, 0.79f, 0.72f, 0.75f), false);
        CreatePanel(parent, "Scale Base", new Vector2(0f, -178f), new Vector2(300f, 44f), new Color(0.52f, 0.57f, 0.49f), true);

        controller.beam = CreatePanel(parent, "Scale Beam", new Vector2(0f, 75f), new Vector2(890f, 26f), new Color(0.58f, 0.36f, 0.20f), true);
        CreatePanel(controller.beam, "Beam Top Edge", new Vector2(0f, 9f), new Vector2(850f, 5f), new Color(0.78f, 0.52f, 0.31f), false);

        controller.leftPan = CreatePanel(parent, "Left Pan", new Vector2(-370f, -25f), new Vector2(290f, 74f), new Color(0.82f, 0.84f, 0.77f), true);
        controller.rightPan = CreatePanel(parent, "Right Pan", new Vector2(370f, -25f), new Vector2(290f, 74f), new Color(0.82f, 0.84f, 0.77f), true);
        CreateText(controller.leftPan, "Left Label", "왼쪽 접시", new Vector2(0f, 0f), new Vector2(240f, 28f), 19, Ink, TextAnchor.MiddleCenter);
        CreateText(controller.rightPan, "Right Label", "오른쪽 접시", new Vector2(0f, 0f), new Vector2(240f, 28f), 19, Ink, TextAnchor.MiddleCenter);

        controller.leftPanContent = CreateDropContent(controller, controller.leftPan, "Left Pan Drop", ScalePuzzleController.Zone.LeftPan, new Vector2(0f, 80f), new Vector2(350f, 92f), false);
        controller.rightPanContent = CreateDropContent(controller, controller.rightPan, "Right Pan Drop", ScalePuzzleController.Zone.RightPan, new Vector2(0f, 80f), new Vector2(350f, 92f), false);

        RectTransform answerTray = CreatePanel(parent, "Answer Tray", new Vector2(555f, -95f), new Vector2(180f, 138f), new Color(0.40f, 0.48f, 0.38f), true);
        CreateText(answerTray, "Answer Tray Label", "정답", new Vector2(0f, 48f), new Vector2(140f, 28f), 20, Color.white, TextAnchor.MiddleCenter);
        controller.answerContent = CreateDropContent(controller, answerTray, "Answer Drop", ScalePuzzleController.Zone.Answer, new Vector2(0f, -18f), new Vector2(120f, 80f), false);
    }

    private static RectTransform CreateDropContent(ScalePuzzleController controller, Transform parent, string name, ScalePuzzleController.Zone zone, Vector2 position, Vector2 size, bool grid)
    {
        RectTransform rect = CreatePanel(parent, name, position, size, new Color(1f, 1f, 1f, 0.035f), false, true);
        ScalePuzzleDropZone dropZone = rect.gameObject.AddComponent<ScalePuzzleDropZone>();
        dropZone.controller = controller;
        dropZone.zone = zone;

        if (grid)
        {
            GridLayoutGroup gridLayout = rect.gameObject.AddComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(72f, 72f);
            gridLayout.spacing = new Vector2(14f, 10f);
            gridLayout.childAlignment = TextAnchor.MiddleCenter;
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = 6;
        }
        else
        {
            HorizontalLayoutGroup layout = rect.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
        }

        return rect;
    }

    private static ScalePuzzleWeightItem[] CreateWeights(ScalePuzzleController controller, GameObject canvas, RectTransform pool)
    {
        ScalePuzzleWeightItem[] items = new ScalePuzzleWeightItem[12];
        for (int i = 0; i < items.Length; i++)
        {
            GameObject token = new GameObject($"{i + 1} Weight Token");
            token.transform.SetParent(pool, false);
            RectTransform rect = token.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(68f, 68f);

            Image body = token.AddComponent<Image>();
            body.color = new Color(0.24f, 0.25f, 0.29f);
            body.raycastTarget = true;
            if (!LowEndUi)
            {
                Shadow shadow = token.AddComponent<Shadow>();
                shadow.effectColor = new Color(0f, 0f, 0f, 0.45f);
                shadow.effectDistance = new Vector2(4f, -4f);
            }
            token.AddComponent<CanvasGroup>();

            GameObject rim = new GameObject("Rim");
            rim.transform.SetParent(token.transform, false);
            RectTransform rimRect = rim.AddComponent<RectTransform>();
            rimRect.anchorMin = new Vector2(0.14f, 0.76f);
            rimRect.anchorMax = new Vector2(0.86f, 0.96f);
            rimRect.offsetMin = Vector2.zero;
            rimRect.offsetMax = Vector2.zero;
            Image rimImage = rim.AddComponent<Image>();
            rimImage.color = new Color(0.56f, 0.57f, 0.62f);
            rimImage.raycastTarget = false;

            Text number = CreateText(rect, "Number", (i + 1).ToString(), new Vector2(0f, -4f), new Vector2(64f, 58f), 28, Color.white, TextAnchor.MiddleCenter);
            if (!LowEndUi)
            {
                Outline outline = number.gameObject.AddComponent<Outline>();
                outline.effectColor = new Color(0f, 0f, 0f, 0.45f);
                outline.effectDistance = new Vector2(1f, -1f);
            }

            ScalePuzzleWeightItem item = token.AddComponent<ScalePuzzleWeightItem>();
            item.numberText = number;
            item.bodyImage = body;
            item.rimImage = rimImage;
            items[i] = item;
        }

        return items;
    }

    private static Camera CreateCamera()
    {
        GameObject cameraGo = new GameObject("Main Camera");
        cameraGo.tag = "MainCamera";
        Camera camera = cameraGo.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Background;
        camera.orthographic = true;
        camera.orthographicSize = 540f;
        camera.enabled = false;
        return camera;
    }

    private static GameObject CreateCanvas()
    {
        GameObject canvasGo = new GameObject("Canvas");
        RectTransform rect = canvasGo.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(1920f, 1080f);
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasGo.AddComponent<GraphicRaycaster>();
        return canvasGo;
    }

    private static RectTransform CreateRect(Transform parent, string name, Vector2 position, Vector2 size)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        return rect;
    }

    private static RectTransform CreatePanel(Transform parent, string name, Vector2 position, Vector2 size, Color color, bool addShadow, bool raycastTarget = false)
    {
        RectTransform rect = CreateRect(parent, name, position, size);
        Image image = rect.gameObject.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = raycastTarget;
        if (addShadow && !LowEndUi)
        {
            Shadow shadow = rect.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.35f);
            shadow.effectDistance = new Vector2(5f, -5f);
        }

        return rect;
    }

    private static Text CreateText(Transform parent, string name, string text, Vector2 position, Vector2 size, int fontSize, Color color, TextAnchor alignment)
    {
        RectTransform rect = CreateRect(parent, name, position, size);
        Text uiText = rect.gameObject.AddComponent<Text>();
        uiText.text = text;
        uiText.fontSize = fontSize;
        uiText.alignment = alignment;
        uiText.color = color;
        uiText.raycastTarget = false;
        uiText.horizontalOverflow = HorizontalWrapMode.Wrap;
        uiText.verticalOverflow = VerticalWrapMode.Overflow;
        Font font = GetKoreanFont();
        if (font != null)
        {
            uiText.font = font;
        }

        return uiText;
    }

    private static Font GetKoreanFont()
    {
        if (koreanFont == null)
        {
            koreanFont = AssetDatabase.LoadAssetAtPath<Font>(FontCopyPath);
        }

        return koreanFont;
    }

    private static void EnsureKoreanFontAsset()
    {
        if (!File.Exists(FontCopyPath))
        {
            if (!File.Exists(FontSourcePath))
            {
                Debug.LogWarning($"Korean font source not found: {FontSourcePath}");
                return;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(FontCopyPath));
            File.Copy(FontSourcePath, FontCopyPath, false);
            AssetDatabase.ImportAsset(FontCopyPath, ImportAssetOptions.ForceSynchronousImport);
        }

        koreanFont = AssetDatabase.LoadAssetAtPath<Font>(FontCopyPath);
        if (koreanFont == null)
        {
            Debug.LogWarning($"Failed to load Korean font asset: {FontCopyPath}");
        }
    }

    private static Button CreateButton(Transform parent, string name, string text, Vector2 position, Vector2 size, int fontSize, Color color)
    {
        RectTransform rect = CreatePanel(parent, name, position, size, color, true, true);
        Button button = rect.gameObject.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = Color.Lerp(color, Color.white, 0.20f);
        colors.pressedColor = Color.Lerp(color, Color.black, 0.12f);
        colors.selectedColor = colors.highlightedColor;
        colors.disabledColor = new Color(0.35f, 0.36f, 0.36f, 0.7f);
        button.colors = colors;
        CreateText(rect, "Text", text, Vector2.zero, size, fontSize, Ink, TextAnchor.MiddleCenter);
        return button;
    }

    private static void CreateEventSystem()
    {
        GameObject eventGo = new GameObject("EventSystem");
        eventGo.AddComponent<EventSystem>();
        eventGo.AddComponent<InputSystemUIInputModule>();
    }

    private static void AddSceneToBuildSettings()
    {
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        if (scenes.Any(scene => scene.path == ScenePath))
        {
            return;
        }

        EditorBuildSettings.scenes = scenes.Concat(new[] { new EditorBuildSettingsScene(ScenePath, true) }).ToArray();
    }
}
