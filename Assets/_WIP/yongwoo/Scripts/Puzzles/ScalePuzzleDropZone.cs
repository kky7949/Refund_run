using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScalePuzzleDropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ScalePuzzleController controller;
    public ScalePuzzleController.Zone zone;
    public Color hoverColor = new Color(0.82f, 0.92f, 1f, 0.32f);

    private Image zoneImage;
    private Color idleColor;

    private void Awake()
    {
        zoneImage = GetComponent<Image>();
        if (zoneImage != null)
        {
            idleColor = zoneImage.color;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsDraggingWeight(eventData) || zoneImage == null)
        {
            return;
        }

        zoneImage.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        RestoreColor();
    }

    public void OnDrop(PointerEventData eventData)
    {
        RestoreColor();

        ScalePuzzleWeightItem item = eventData.pointerDrag != null
            ? eventData.pointerDrag.GetComponent<ScalePuzzleWeightItem>()
            : null;

        if (item == null || controller == null)
        {
            return;
        }

        controller.PlaceWeight(item.Index, zone);
    }

    private static bool IsDraggingWeight(PointerEventData eventData)
    {
        return eventData.pointerDrag != null
            && eventData.pointerDrag.GetComponent<ScalePuzzleWeightItem>() != null;
    }

    private void RestoreColor()
    {
        if (zoneImage != null)
        {
            zoneImage.color = idleColor;
        }
    }
}
