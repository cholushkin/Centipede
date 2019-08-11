using UnityEngine;

public abstract class BoardEntityBase : MonoBehaviour, IBoardEntity
{
    public BoardController Board;
    protected Vector2Int _boardPosition;

    public abstract GameConstants.CellType GetCellType();

    public virtual Vector2Int GetBoardPosition()
    {
        return _boardPosition;
    }

    public virtual void SetBoardPosition(Vector2Int pos, bool clearPrevPosition = true)
    {
        if(clearPrevPosition)
            Board.CellAccessor.Set(_boardPosition, null);
        _boardPosition = pos;
        Board.CellAccessor.Set(_boardPosition, this);
    }

    public abstract void Remove();
}
