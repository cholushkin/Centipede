using UnityEngine;

public abstract class BoardEntityBase : MonoBehaviour, IBoardEntity
{
    public BoardController Board;
    private Vector2Int _boardPosition;

    public abstract GameConstants.CellType GetCellType();

    public virtual Vector2Int GetPosition()
    {
        return _boardPosition;
    }

    public void SetPosition(Vector2Int pos, bool clearPrevPosition = true)
    {
        if(clearPrevPosition)
            Board.CellAccessor.Set(_boardPosition, null);
        _boardPosition = pos;
        Board.CellAccessor.Set(_boardPosition, this);
    }
}
