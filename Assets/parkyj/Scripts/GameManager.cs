using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public LevelData[] levels;
    private int currentLevel = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        LoadLevel(currentLevel);
    }

    void LoadLevel(int idx)
    {
        LevelData level = levels[idx];
        GridBoard.Instance.Initialize(level);
        BlockSpawner.Instance.SpawnPieces(level.pieces);
    }

    public void OnPuzzleSolved()
    {
        Debug.Log("Puzzle Solved!");
        UIManager.Instance.ShowClear();
    }
}