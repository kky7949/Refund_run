using UnityEngine;

public class GridBoard : MonoBehaviour
{
    public static GridBoard Instance;

    public GameObject cellPrefab;
    public float cellSize = 1.0f;

    private int width;
    private int height;
    private int[,] grid;
    private GridCell[,] cells;

    void Awake()
    {
        Instance = this;
    }

    public void Initialize(LevelData level)
    {
        width = level.boardWidth;
        height = level.boardHeight;
        grid = new int[width, height];
        cells = new GridCell[width, height];

        // 격자를 화면 중앙에 배치
        float offsetX = -(width - 1) * cellSize / 2f;
        float offsetY = (height - 1) * cellSize / 2f;
        transform.position = new Vector3(offsetX, offsetY, 0);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int idx = y * width + x;
                grid[x, y] = level.targetShape[idx];

                Vector3 pos = GetWorldPos(x, y);
                GameObject go = Instantiate(cellPrefab, pos, Quaternion.identity, transform);
                cells[x, y] = go.GetComponent<GridCell>();
                cells[x, y].Init(x, y, grid[x, y] == 1);
            }
        }
    }

    public bool CanPlace(int[,] pieceShape, int px, int py, int pw, int ph)
    {
        for (int y = 0; y < ph; y++)
        {
            for (int x = 0; x < pw; x++)
            {
                if (pieceShape[x, y] == 0) continue;

                int gx = px + x;
                int gy = py + y;

                if (gx < 0 || gx >= width || gy < 0 || gy >= height)
                {
                    Debug.Log($"범위 초과: gx={gx}, gy={gy}");
                    return false;
                }
                if (grid[gx, gy] != 1)
                {
                    Debug.Log($"실루엣 아님: grid[{gx},{gy}]={grid[gx, gy]}");
                    return false;
                }
            }
        }
        return true;
    }

    public void PlacePiece(int[,] pieceShape, int px, int py, int pw, int ph, Color color)
    {
        for (int y = 0; y < ph; y++)
        {
            for (int x = 0; x < pw; x++)
            {
                if (pieceShape[x, y] == 0) continue;

                int gx = px + x;
                int gy = py + y;
                grid[gx, gy] = 2;
                cells[gx, gy].SetFilled(color);
            }
        }
        PuzzleChecker.Instance.Check(grid, width, height);
    }

    public void RemovePiece(int[,] pieceShape, int px, int py, int pw, int ph)
    {
        for (int y = 0; y < ph; y++)
        {
            for (int x = 0; x < pw; x++)
            {
                if (pieceShape[x, y] == 0) continue;

                int gx = px + x;
                int gy = py + y;
                grid[gx, gy] = 1;
                cells[gx, gy].SetSilhouette();
            }
        }
    }

    public void HighlightCell(int x, int y, bool canPlace)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            cells[x, y].Highlight(canPlace);
        }
    }

    public void ClearAllHighlights()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                cells[x, y].ResetHighlight();
            }
        }
    }

    public Vector3 GetWorldPos(int x, int y)
    {
        return transform.position + new Vector3(x * cellSize, -y * cellSize, 0);
    }

    public bool GetGridPos(Vector3 worldPos, out int gx, out int gy)
    {
        Vector3 local = worldPos - transform.position;
        gx = Mathf.RoundToInt(local.x / cellSize);
        gy = Mathf.RoundToInt(-local.y / cellSize);

        Debug.Log($"worldPos={worldPos}, boardPos={transform.position}, gx={gx}, gy={gy}, width={width}, height={height}");

        return gx >= 0 && gx < width && gy >= 0 && gy < height;
    }
}