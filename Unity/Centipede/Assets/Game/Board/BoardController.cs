using UnityEngine;

public class BoardController : MonoBehaviour
{
    public CellAccessor CellAccessor;
    public Transform TransformMushrooms;
    public Transform TransformEnemies;

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

    public void PurgeEnemies()
    {
        // delete old
        Destroy(TransformEnemies.gameObject);

        // create new
        var newEnemiesOwner = new GameObject(TransformEnemies.name);
        newEnemiesOwner.transform.SetParent(transform);
        TransformEnemies = newEnemiesOwner.transform;
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
                Gizmos.color = Color.black;
                if (CellAccessor.Get(x,y).CellType == GameConstants.CellType.Empty)
                    Gizmos.color = Color.white;
                if (CellAccessor.Get(x, y).CellType == GameConstants.CellType.Centipede)
                    Gizmos.color = Color.magenta;
                if (CellAccessor.Get(x, y).CellType == GameConstants.CellType.Player)
                    Gizmos.color = Color.yellow;

                Gizmos.DrawCube(new Vector3(x * GameConstants.CellWidth, y * GameConstants.CellHeight, 0), Vector3.one * 0.02f);
            }
    }
}
