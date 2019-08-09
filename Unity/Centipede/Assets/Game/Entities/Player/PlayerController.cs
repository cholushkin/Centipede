using UnityEngine;
using UnityEngine.Assertions;

public class PlayerController : BoardEntityBase
{
    public class EventPlayerDie
    {
    }

    public enum State
    {
        Alive,
        Dead
    }
    public Weapon Weapon;
    public PlayerVisual Visual;
    public bool IsSlideModeMovement;
    public float Speed;
    public Follower Follower;

    private Vector3 _positionPointer;
    private int _maxTopY;
    private State _currentState;
    private float _godModeTimer;


    void Awake()
    {
        // enter state 'alive'  // todo: rewrite using state machine
        _currentState = State.Alive;
        _positionPointer = transform.position;
        SwitchToGodMode();
    }

    void Update()
    {
        if (_currentState == State.Dead)
            return;

        ProcessInput();

        // process god mode
        _godModeTimer -= Time.deltaTime;
    }

    public void SetActiveArea(int maxTopY)
    {
        _maxTopY = maxTopY;
    }

    private void ProcessInput()
    {
        // shoot
        if (Input.GetKey(KeyCode.Space))
        {
            Assert.IsNotNull(Weapon);
            Weapon.Shoot(Vector3.up);
        }

        // move
        {
            var offset = Vector3.zero;
            if (Input.GetKey(KeyCode.LeftArrow))
                offset += Vector3.left;
            if (Input.GetKey(KeyCode.RightArrow))
                offset += Vector3.right;
            if (Input.GetKey(KeyCode.UpArrow))
                offset += Vector3.up;
            if (Input.GetKey(KeyCode.DownArrow))
                offset += Vector3.down;

            bool isMoved;
            if (IsSlideModeMovement)
                isMoved = ProcessMovement(offset)
                          || ProcessMovement(new Vector3(0, offset.y, 0))
                          || ProcessMovement(new Vector3(offset.x, 0, 0));
            else
                isMoved = ProcessMovement(offset);
        }

        // debug seppuku
        if (Input.GetKey(KeyCode.S))
            Die();
    }

    private bool ProcessMovement(Vector3 offset)
    {
        var newPositionPointer = _positionPointer + offset * Speed * Time.deltaTime;
        var newBoardPos = Board.ToBoardPosition(newPositionPointer);
        var cell = Board.CellAccessor.Get(newBoardPos);

        // restrict by screen edges
        var isOut = cell.CellType == GameConstants.CellType.Undefined;
        if(isOut)
            return false;

        // restrict by _maxTopY
        if( newBoardPos.y >= _maxTopY )
            return false;

        // mushroom?
        if (cell.CellType == GameConstants.CellType.Mushroom)
            return false;

        _positionPointer = newPositionPointer;
        Follower.Follow(_positionPointer);
        return true;
    }

    private void Die()
    {
        GlobalEventAggregator.EventAggregator.Publish(new EventPlayerDie());
        _currentState = State.Dead;
        Visual.PlayDead();
    }

    void SwitchToGodMode()
    {
        _godModeTimer = GameConstants.GodModeTimer;
        Visual.PlayGodMode();
    }

    bool IsInGodMode()
    {
        return _godModeTimer >= 0f;
    }

    #region BoardEntityBase
    public override GameConstants.CellType GetCellType()
    {
        return GameConstants.CellType.Player;
    }
    #endregion
}
