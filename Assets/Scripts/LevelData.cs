using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Puzzle/Level Data")]
public class LevelData : ScriptableObject
{
    public int boardWidth = 5;
    public int boardHeight = 5;
    public int[] targetShape;
    public PieceData[] pieces;
}

[System.Serializable]
public class PieceData
{
    public int width;
    public int height;
    public int[] shape;
    public Color color;
}