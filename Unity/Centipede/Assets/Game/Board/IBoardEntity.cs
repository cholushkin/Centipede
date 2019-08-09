using UnityEngine;

public interface IBoardEntity
{
    GameConstants.CellType GetCellType();
    Vector2Int GetPosition();
    void SetPosition(Vector2Int pos, bool clearPrevPosition);
}
