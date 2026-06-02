using UnityEngine;

public class GridCell : MonoBehaviour
{
    private SpriteRenderer sr;
    private bool isSilhouette;

    public Color emptyColor = new Color(0.15f, 0.15f, 0.2f, 1f);      // 빈칸 (어두운 색)
    public Color silhouetteColor = new Color(0.6f, 0.45f, 0.25f, 1f); // 실루엣 (나무색)

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Init(int x, int y, bool silhouette)
    {
        isSilhouette = silhouette;
        sr.color = silhouette ? silhouetteColor : emptyColor;
    }

    public void SetFilled(Color color)
    {
        sr.color = color;
    }

    public void SetSilhouette()
    {
        sr.color = silhouetteColor;
        isSilhouette = true;
    }

    public void Highlight(bool canPlace)
    {
        sr.color = canPlace ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 0f, 0f, 0.5f);
    }

    public void ResetHighlight()
    {
        sr.color = isSilhouette ? silhouetteColor : emptyColor;
    }
}