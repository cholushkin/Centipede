using System;
using UnityEngine;

public class Mushroom : BoardEntityBase
{
    public MushroomVisual Visual;

    public float Hitpoints;
    private float _currentHitpoints;

    public void Init(float hitPoints, Vector2Int boardPos, BoardController board)
    {
        Board = board;
        Hitpoints = _currentHitpoints = hitPoints;
        SetBoardPosition(boardPos, false);
        transform.position = Board.ToWorldPosition(boardPos);
    }

    public void ApplyDamage(float damage)
    {
        _currentHitpoints -= damage;
        Visual.ShowDamage(1f - _currentHitpoints / Hitpoints);
        if (_currentHitpoints < 0f)
            Remove();
    }
    
    #region BoardEntityBase
    public override GameConstants.CellType GetCellType()
    {
        return GameConstants.CellType.Mushroom;
    }

    public override void Remove()
    {
        Board.CellAccessor.Set(_boardPosition, null);
        Destroy(gameObject);
    }
    #endregion
}