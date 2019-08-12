using UnityEngine;

namespace Game
{
    public interface IBoardEntity
    {
        GameConstants.CellType GetCellType();
        Vector2Int GetBoardPosition();
        void SetBoardPosition(Vector2Int pos, bool clearPrevPosition);
        void Remove();
    }
}