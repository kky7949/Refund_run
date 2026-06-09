using UnityEngine;
using UnityEngine.InputSystem;

public class BlockPiece : MonoBehaviour
{
    public PieceData data;

    private int[,] shape2D;
    private Vector3 originalPos;
    private bool isPlaced = false;
    private bool isDragging = false;
    private Vector3 mouseOffset;
    private Camera mainCam;

    public void Init(PieceData pieceData)
    {
        data = pieceData;
        shape2D = To2D(data.shape, data.width, data.height);
        originalPos = transform.position;
        mainCam = Camera.main;
        RenderBlock();
    }

    void RenderBlock()
    {
        for (int y = 0; y < data.height; y++)
        {
            for (int x = 0; x < data.width; x++)
            {
                if (shape2D[x, y] == 0) continue;

                GameObject cell = new GameObject("cell_" + x + "_" + y);
                cell.transform.SetParent(transform);
                cell.transform.localPosition =
                    new Vector3(x, -y, 0) * GridBoard.Instance.cellSize;

                SpriteRenderer sr = cell.AddComponent<SpriteRenderer>();
                sr.sprite = CreateSquareSprite();
                sr.color = data.color;
                sr.sortingOrder = 10;
            }
        }

        BoxCollider2D col = gameObject.AddComponent<BoxCollider2D>();
        col.size = new Vector2(data.width, data.height) * GridBoard.Instance.cellSize;
        col.offset = new Vector2(
            (data.width - 1) * GridBoard.Instance.cellSize / 2f,
            -(data.height - 1) * GridBoard.Instance.cellSize / 2f
        );
    }

    void Update()
    {
        if (isPlaced) return;

        var mouse = Mouse.current;
        if (mouse == null) return;

        Vector3 mouseWorld = GetMouseWorld();

        if (mouse.leftButton.wasPressedThisFrame)
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null && col.OverlapPoint(mouseWorld))
            {
                isDragging = true;
                mouseOffset = transform.position - mouseWorld;
                foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
                    sr.sortingOrder = 20;
            }
        }

        if (mouse.leftButton.isPressed && isDragging)
        {
            transform.position = mouseWorld + mouseOffset;

            GridBoard.Instance.ClearAllHighlights();

            // 블록의 왼쪽 위 셀 기준으로 격자 위치 계산
            Vector3 topLeft = transform.position;
            if (GridBoard.Instance.GetGridPos(topLeft, out int gx, out int gy))
            {
                bool canPlace = GridBoard.Instance.CanPlace(
                    shape2D, gx, gy, data.width, data.height);
                for (int y = 0; y < data.height; y++)
                    for (int x = 0; x < data.width; x++)
                    {
                        if (shape2D[x, y] == 0) continue;
                        GridBoard.Instance.HighlightCell(gx + x, gy + y, canPlace);
                    }
            }
        }

        if (mouse.leftButton.wasReleasedThisFrame && isDragging)
        {
            isDragging = false;
            GridBoard.Instance.ClearAllHighlights();

            Vector3 topLeft = transform.position;
            if (GridBoard.Instance.GetGridPos(topLeft, out int gx, out int gy) &&
                GridBoard.Instance.CanPlace(shape2D, gx, gy, data.width, data.height))
            {
                // 격자에 스냅
                transform.position = GridBoard.Instance.GetWorldPos(gx, gy);
                GridBoard.Instance.PlacePiece(
                    shape2D, gx, gy, data.width, data.height, data.color);
                isPlaced = true;

                foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
                    sr.sortingOrder = 10;
            }
            else
            {
                transform.position = originalPos;
                foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
                    sr.sortingOrder = 10;
            }
        }
    }

    Vector3 GetMouseWorld()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mp = new Vector3(mouseScreenPos.x, mouseScreenPos.y,
            Mathf.Abs(mainCam.transform.position.z));
        return mainCam.ScreenToWorldPoint(mp);
    }

    Sprite CreateSquareSprite()
    {
        Texture2D tex = new Texture2D(32, 32);
        Color[] pixels = new Color[32 * 32];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 32, 32),
            new Vector2(0.5f, 0.5f), 32);
    }

    int[,] To2D(int[] flat, int w, int h)
    {
        int[,] result = new int[w, h];
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                result[x, y] = flat[y * w + x];
        return result;
    }
}