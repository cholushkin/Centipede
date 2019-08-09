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
                break;
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
                break;
            }
            case GameConstants.CellType.Wall:
            {
                break;
            }
            case GameConstants.CellType.Undefined:
            {
                Die();
                return;
            }
        }

        //const float CollisionFactor = 0.25f;
        //var enemies = Enemies.GetComponentsInChildren<BaseEnemy>();
        //foreach (var enemy in enemies)
        //{
        //    var distance = (enemy.transform.position - transform.position).sqrMagnitude;
        //    if (distance < CollisionFactor)
        //    {
        //        enemy.ReceiveDamage(Damage);
        //        Destroy(gameObject);
        //        return;
        //    }
        //}
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}