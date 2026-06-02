using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ScalePuzzleWeightItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Text numberText;
    public Image bodyImage;
    public Image rimImage;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private ScalePuzzleController controller;
    private int index;

    public int Index => index;

    private void Awake()
    {
        EnsureReferences();
    }

    public void Initialize(ScalePuzzleController owner, int itemIndex)
    {
        EnsureReferences();
        controller = owner;
        index = itemIndex;
        SetNumber(itemIndex + 1);
    }

    public void SetNumber(int number)
    {
        if (numberText != null)
        {
            numberText.text = number.ToString();
        }
    }

    public void SetZone(ScalePuzzleController.Zone zone)
    {
        Color baseColor = zone switch
        {
            ScalePuzzleController.Zone.LeftPan => new Color(0.48f, 0.68f, 0.96f),
            ScalePuzzleController.Zone.RightPan => new Color(0.95f, 0.64f, 0.34f),
            ScalePuzzleController.Zone.Answer => new Color(0.49f, 0.86f, 0.48f),
            _ => new Color(0.24f, 0.25f, 0.29f)
        };

        if (bodyImage != null)
        {
            bodyImage.color = baseColor;
        }

        if (rimImage != null)
        {
            rimImage.color = Color.Lerp(baseColor, Color.white, 0.35f);
        }
    }

    public void SnapTo(RectTransform parent)
    {
        EnsureReferences();
        if (parent == null)
        {
            return;
        }

        transform.SetParent(parent, false);
        rectTransform.localScale = Vector3.one;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (controller == null || !controller.CanDrag(index))
        {
            return;
        }

        transform.SetParent(controller.GetDragLayer(), false);
        transform.SetAsLastSibling();
        EnsureReferences();
        canvasGroup.alpha = 0.86f;
        canvasGroup.blocksRaycasts = false;
        rectTransform.localScale = Vector3.one * 1.08f;
        MoveToPointer(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (controller == null || !controller.CanDrag(index))
        {
            return;
        }

        MoveToPointer(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (controller == null)
        {
            return;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        controller.SnapWeight(index);
    }

    private void EnsureReferences()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
    }

    private void MoveToPointer(PointerEventData eventData)
    {
        EnsureReferences();

        RectTransform parentRect = rectTransform.parent as RectTransform;
        if (parentRect == null)
        {
            return;
        }

        Camera eventCamera = eventData.pressEventCamera != null
            ? eventData.pressEventCamera
            : eventData.enterEventCamera;

        Canvas canvas = parentRect.GetComponentInParent<Canvas>();
        if (eventCamera == null && canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            eventCamera = canvas.worldCamera;
        }

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(parentRect, eventData.position, eventCamera, out Vector3 worldPoint))
        {
            rectTransform.position = worldPoint;
        }
    }
}
