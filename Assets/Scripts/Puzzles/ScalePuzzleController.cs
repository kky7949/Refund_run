using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ScalePuzzleController : MonoBehaviour
{
    public event Action<ScalePuzzleController> SolvedCorrectly;

    public enum WeightKind
    {
        Lighter,
        Heavier
    }

    public enum Zone
    {
        Pool,
        LeftPan,
        RightPan,
        Answer
    }

    [Serializable]
    public sealed class PuzzleTemplate
    {
        public string title;
        public int weightCount = 8;
        public WeightKind oddWeightKind = WeightKind.Lighter;
        public int maxWeighs = 2;
    }

    [Header("Puzzle")]
    public PuzzleTemplate[] templates = Array.Empty<PuzzleTemplate>();
    public ScalePuzzleWeightItem[] weights = Array.Empty<ScalePuzzleWeightItem>();
    public int normalWeight = 10;
    public int oddWeightOffset = 1;

    [Header("Drop Zones")]
    public RectTransform poolContent;
    public RectTransform leftPanContent;
    public RectTransform rightPanContent;
    public RectTransform answerContent;
    public RectTransform dragLayer;

    [Header("Scale Visuals")]
    public RectTransform beam;
    public RectTransform leftPan;
    public RectTransform rightPan;
    public float tiltPerWeight = 5f;
    public float maxTilt = 16f;
    public float panDropPerWeight = 7f;

    [Header("UI")]
    public Text puzzleText;
    public Text remainingText;
    public Text feedbackText;
    public Text historyText;
    public Text answerText;
    public Button weighButton;
    public Button decideButton;
    public Button resetButton;
    public Button newPuzzleButton;

    private readonly Zone[] placements = new Zone[12];
    private readonly System.Random random = new System.Random();
    private readonly StringBuilder history = new StringBuilder();
    private PuzzleTemplate currentTemplate;
    private Vector2 leftPanStart;
    private Vector2 rightPanStart;
    private int oddIndex;
    private int remainingWeighs;
    private int answerIndex = -1;
    private int lastDelta;
    private bool solved;

    private void Awake()
    {
        leftPanStart = leftPan != null ? leftPan.anchoredPosition : Vector2.zero;
        rightPanStart = rightPan != null ? rightPan.anchoredPosition : Vector2.zero;

        for (int i = 0; i < weights.Length; i++)
        {
            weights[i].Initialize(this, i);
        }

        weighButton.onClick.AddListener(Weigh);
        decideButton.onClick.AddListener(Decide);
        resetButton.onClick.AddListener(ResetCurrentPuzzle);
        newPuzzleButton.onClick.AddListener(NewPuzzle);

        NewPuzzle();
    }

    public bool CanDrag(int index)
    {
        return !solved && currentTemplate != null && index >= 0 && index < currentTemplate.weightCount;
    }

    public RectTransform GetDragLayer()
    {
        return dragLayer;
    }

    public void PlaceWeight(int index, Zone zone)
    {
        if (!CanDrag(index))
        {
            SnapWeight(index);
            return;
        }

        if (zone == Zone.Answer)
        {
            for (int i = 0; i < currentTemplate.weightCount; i++)
            {
                if (i != index && placements[i] == Zone.Answer)
                {
                    placements[i] = Zone.Pool;
                    SnapWeight(i);
                }
            }

            answerIndex = index;
        }
        else if (answerIndex == index)
        {
            answerIndex = -1;
        }

        placements[index] = zone;
        feedbackText.text = "추를 올렸어. 양쪽 접시 개수를 맞춘 뒤 저울질을 눌러.";
        UpdateAll();
    }

    public void SnapWeight(int index)
    {
        if (index < 0 || index >= weights.Length || weights[index] == null)
        {
            return;
        }

        RectTransform parent = GetZoneContent(placements[index]);
        weights[index].SnapTo(parent);
    }

    public void NewPuzzle()
    {
        if (templates == null || templates.Length == 0)
        {
            templates = new[]
            {
                new PuzzleTemplate { title = "8개의 추 중 하나만 더 가볍습니다.", weightCount = 8, oddWeightKind = WeightKind.Lighter, maxWeighs = 2 }
            };
        }

        currentTemplate = templates[random.Next(templates.Length)];
        oddIndex = random.Next(currentTemplate.weightCount);
        remainingWeighs = currentTemplate.maxWeighs;
        solved = false;
        lastDelta = 0;
        answerIndex = -1;
        history.Clear();

        for (int i = 0; i < placements.Length; i++)
        {
            placements[i] = Zone.Pool;
        }

        feedbackText.text = "추를 양쪽 접시에 같은 개수로 올려서 비교해.";
        UpdateAll();
    }

    public void ResetCurrentPuzzle()
    {
        if (currentTemplate == null)
        {
            NewPuzzle();
            return;
        }

        remainingWeighs = currentTemplate.maxWeighs;
        solved = false;
        lastDelta = 0;
        answerIndex = -1;
        history.Clear();

        for (int i = 0; i < currentTemplate.weightCount; i++)
        {
            placements[i] = Zone.Pool;
        }

        feedbackText.text = "초기화했어. 숨겨진 정답 추는 그대로야.";
        UpdateAll();
    }

    public void Weigh()
    {
        if (solved || currentTemplate == null)
        {
            return;
        }

        if (remainingWeighs <= 0)
        {
            feedbackText.text = "저울질 기회를 모두 썼어. 정답 칸에 추 하나를 올리고 결정해.";
            UpdateAll();
            return;
        }

        List<int> left = GetIndexes(Zone.LeftPan);
        List<int> right = GetIndexes(Zone.RightPan);
        if (left.Count == 0 || left.Count != right.Count)
        {
            feedbackText.text = "양쪽 접시에 같은 개수의 추를 올려야 해.";
            UpdateAll();
            return;
        }

        int leftTotal = GetTotalWeight(left);
        int rightTotal = GetTotalWeight(right);
        lastDelta = leftTotal - rightTotal;
        remainingWeighs--;

        string result = lastDelta == 0 ? "수평" : lastDelta > 0 ? "왼쪽 무거움" : "오른쪽 무거움";
        history.AppendLine($"{currentTemplate.maxWeighs - remainingWeighs}) 왼쪽:{FormatIndexes(left)} / 오른쪽:{FormatIndexes(right)} -> {result}");
        feedbackText.text = ResultText(lastDelta);
        UpdateAll();
    }

    public void Decide()
    {
        if (solved || currentTemplate == null)
        {
            return;
        }

        if (answerIndex < 0)
        {
            feedbackText.text = "정답 칸에 추 하나를 먼저 올려.";
            UpdateAll();
            return;
        }

        bool correct = answerIndex == oddIndex;
        if (!correct)
        {
            feedbackText.text = $"오답. 정답은 {oddIndex + 1}번 {OddKindText()} 추가 맞아.";
            UpdateAll();
            return;
        }

        solved = true;
        feedbackText.text = $"정답. {answerIndex + 1}번이 {OddKindText()} 추가 맞아.";
        UpdateAll();
        SolvedCorrectly?.Invoke(this);
    }

    private RectTransform GetZoneContent(Zone zone)
    {
        return zone switch
        {
            Zone.LeftPan => leftPanContent,
            Zone.RightPan => rightPanContent,
            Zone.Answer => answerContent,
            _ => poolContent
        };
    }

    private int GetActualWeight(int index)
    {
        if (index != oddIndex)
        {
            return normalWeight;
        }

        return currentTemplate.oddWeightKind == WeightKind.Lighter
            ? normalWeight - oddWeightOffset
            : normalWeight + oddWeightOffset;
    }

    private int GetTotalWeight(List<int> indexes)
    {
        int total = 0;
        foreach (int index in indexes)
        {
            total += GetActualWeight(index);
        }

        return total;
    }

    private List<int> GetIndexes(Zone zone)
    {
        List<int> result = new List<int>();
        for (int i = 0; i < currentTemplate.weightCount; i++)
        {
            if (placements[i] == zone)
            {
                result.Add(i);
            }
        }

        return result;
    }

    private string FormatIndexes(List<int> indexes)
    {
        if (indexes.Count == 0)
        {
            return "-";
        }

        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < indexes.Count; i++)
        {
            if (i > 0)
            {
                builder.Append(",");
            }

            builder.Append(indexes[i] + 1);
        }

        return builder.ToString();
    }

    private string OddKindText()
    {
        return currentTemplate.oddWeightKind == WeightKind.Lighter ? "가벼운" : "무거운";
    }

    private string ResultText(int delta)
    {
        if (delta == 0)
        {
            return "저울이 수평이야.";
        }

        return delta > 0 ? "왼쪽 접시가 더 무거워." : "오른쪽 접시가 더 무거워.";
    }

    private void UpdateAll()
    {
        if (currentTemplate == null)
        {
            return;
        }

        puzzleText.text = $"{currentTemplate.weightCount}개의 추 중 하나만 {OddKindText()} 추입니다.\n{currentTemplate.maxWeighs}번 안에 정답 추를 찾으세요.";
        remainingText.text = $"남은 저울질: {remainingWeighs}/{currentTemplate.maxWeighs}";
        answerText.text = answerIndex < 0 ? "정답: 없음" : $"정답: {answerIndex + 1}번";
        historyText.text = history.Length == 0 ? "기록: 없음" : history.ToString();

        UpdateScaleVisual();
        UpdateWeightVisuals();
        weighButton.interactable = !solved && remainingWeighs > 0;
        decideButton.interactable = !solved;
        resetButton.interactable = !solved;
    }

    private void UpdateScaleVisual()
    {
        float tilt = Mathf.Clamp(lastDelta * tiltPerWeight, -maxTilt, maxTilt);

        if (beam != null)
        {
            beam.localRotation = Quaternion.Euler(0f, 0f, tilt);
        }

        if (leftPan != null)
        {
            leftPan.anchoredPosition = leftPanStart + Vector2.down * Mathf.Max(0, lastDelta) * panDropPerWeight;
        }

        if (rightPan != null)
        {
            rightPan.anchoredPosition = rightPanStart + Vector2.down * Mathf.Max(0, -lastDelta) * panDropPerWeight;
        }
    }

    private void UpdateWeightVisuals()
    {
        for (int i = 0; i < weights.Length; i++)
        {
            bool active = i < currentTemplate.weightCount;
            weights[i].gameObject.SetActive(active);
            if (!active)
            {
                continue;
            }

            weights[i].SetNumber(i + 1);
            weights[i].SetZone(placements[i]);
            weights[i].SnapTo(GetZoneContent(placements[i]));
            weights[i].transform.SetSiblingIndex(i);
        }
    }
}
