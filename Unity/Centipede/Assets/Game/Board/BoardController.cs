using UnityEngine;

public class BoardController : MonoBehaviour
{
    public CellAccessor CellAccessor;

    public Vector2Int ToBoardPosition(Vector3 worldPosition)
    {
        return new Vector2Int(
            Mathf.RoundToInt((worldPosition.x / GameConstants.CellWidth)),
            Mathf.RoundToInt((worldPosition.y / GameConstants.CellHeight))
        );
    }

    public Vector3 ToWorldPosition(Vector2Int boardCoordinates)
    {
        return new Vector3(
            boardCoordinates.x * GameConstants.CellWidth,
            boardCoordinates.y * GameConstants.CellHeight,
            0f);
    }

    public void SetGrid(Vector2Int gridSize)
    {
        CellAccessor = new CellAccessor(gridSize);
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        Gizmos.color = Color.red;
        for (int x = 0; x < CellAccessor.BoardSize.x; ++x)
            for (int y = 0; y < CellAccessor.BoardSize.y; ++y)
            {
                Gizmos.DrawCube(new Vector3(x * GameConstants.CellWidth, y * GameConstants.CellHeight, 0), Vector3.one * 0.01f);
            }
    }
}
