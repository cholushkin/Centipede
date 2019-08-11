using UnityEngine;
using UnityEngine.Assertions;

public class Laser : MonoBehaviour
{
    public BoardController Board;

    public float Speed;
    public float Damage;

    private Vector2Int _boardPosition;

    void Update()
    {
        var delta = Speed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, transform.position.y + delta, transform.position.z);
        _boardPosition = Board.ToBoardPosition(transform.position);
        ProcessCollisions();
    }

    private void ProcessCollisions()
    {
        var cell = Board.CellAccessor.Get(_boardPosition);

        switch (cell.CellType)
        {
            case GameConstants.CellType.Centipede:
            {
                var centipede = cell.Entity as Centipede;
                Assert.IsNotNull(cell.Entity);
                Assert.IsNotNull(centipede, cell.Entity.GetType().ToString());
                centipede.ApplyDamage(Damage, _boardPosition);
                Die();
                return;
            }
            case GameConstants.CellType.Mushroom:
            {
                var mushroom = cell.Entity as Mushroom;
                Assert.IsNotNull(mushroom);
                mushroom.ApplyDamage(Damage);
                Die();
                return;
            }
            case GameConstants.CellType.Spider:
            {
                return;
            }
            case GameConstants.CellType.Undefined:
            {
                Die();
                return;
            }
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}