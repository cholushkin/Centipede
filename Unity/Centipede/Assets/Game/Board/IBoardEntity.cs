using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBoardEntity
{
    GameConstants.CellType GetCellType();
    Vector2Int GetPosition();
    Vector2Int SetPosition();
}
