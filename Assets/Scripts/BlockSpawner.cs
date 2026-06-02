using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public static BlockSpawner Instance;

    public float spacingX = 2.5f;   // 블록 사이 가로 간격
    public float spawnY = -5f;      // 격자판 아래 위치

    void Awake()
    {
        Instance = this;
    }

    public void SpawnPieces(PieceData[] pieces)
    {
        // 블록들을 가운데 정렬
        float totalWidth = (pieces.Length - 1) * spacingX;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < pieces.Length; i++)
        {
            GameObject go = new GameObject("Block_" + i);
            go.transform.position = new Vector3(startX + i * spacingX, spawnY, 0);

            BlockPiece bp = go.AddComponent<BlockPiece>();
            bp.Init(pieces[i]);
        }
    }
}