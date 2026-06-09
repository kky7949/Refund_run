using UnityEngine;

public class PuzzleChecker : MonoBehaviour
{
    public static PuzzleChecker Instance;

    void Awake()
    {
        Instance = this;
    }

    public void Check(int[,] grid, int width, int height)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[x, y] == 1)
                {
                    return;
                }
            }
        }

        GameManager.Instance.OnPuzzleSolved();
    }
}