using UnityEngine;
using UnityEngine.Assertions;
using Utils;

namespace Game
{
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
        public float DeathDuration;
        public State CurrentState { get; private set; }

        private Vector3 _positionPointer;
        private int _maxTopY;
        private float _deathTimer;
        private Vector3 _inputMoveOffset;
        private bool _inputIsShooting;
        private bool _isInjuredDuringMovement;

        private void Update()
        {
            if(StateGameplay.Instance.IsWin)
                return;
            // process death
            if (CurrentState == State.Dead)
            {
                _deathTimer -= Time.deltaTime;
                if (_deathTimer < 0f)
                {
                    GlobalEventAggregator.EventAggregator.Publish(new EventPlayerDie());
                    Destroy(gameObject);
                }
            }
            else if (CurrentState == State.Alive)
            {
                ProcessInput();

                if (_inputIsShooting)
                {
                    Assert.IsNotNull(Weapon);
                    Weapon.Shoot(Vector3.up);
                }

                _isInjuredDuringMovement = false;
                var isMoved = false;
                if (IsSlideModeMovement)
                    isMoved = ProcessMovement(_inputMoveOffset) || ProcessMovement(new Vector3(0, _inputMoveOffset.y, 0)) ||
                              ProcessMovement(new Vector3(_inputMoveOffset.x, 0, 0));
                else
                    isMoved = ProcessMovement(_inputMoveOffset);

                if (_isInjuredDuringMovement)
                    Remove();
            }
        }

        public void Init(Vector2Int boardPos, BoardController board, BalanceConfig config)
        {
            CurrentState = State.Alive;
            Board = board;
            Weapon.Board = Board;
            Weapon.BulletSpeed = config.PlayerBulletSpeed;
            Speed = config.PlayerSpeed;
            SetBoardPosition(boardPos, false);
            transform.position = Board.ToWorldPosition(boardPos);
            SetActiveArea(Mathf.RoundToInt(config.GridSize.y * config.ActiveAreaOffsetPercent));
            _positionPointer = transform.position;
        }

        private void SetActiveArea(int maxTopY)
        {
            _maxTopY = maxTopY;
        }

        private void ProcessInput()
        {
            // shoot
            _inputIsShooting = Input.GetKey(KeyCode.Space);

            // move
            _inputMoveOffset = Vector3.zero;
            if (Input.GetKey(KeyCode.LeftArrow))
                _inputMoveOffset += Vector3.left;
            if (Input.GetKey(KeyCode.RightArrow))
                _inputMoveOffset += Vector3.right;
            if (Input.GetKey(KeyCode.UpArrow))
                _inputMoveOffset += Vector3.up;
            if (Input.GetKey(KeyCode.DownArrow))
                _inputMoveOffset += Vector3.down;
        }

        private bool ProcessMovement(Vector3 offset)
        {
            var newPositionPointer = _positionPointer + offset * Speed * Time.deltaTime;
            var newBoardPos = Board.ToBoardPosition(newPositionPointer);
            var cell = Board.CellAccessor.Get(newBoardPos);

            // restrict by screen edges
            var isOut = cell.CellType == GameConstants.CellType.Undefined;
            if (isOut)
                return false;

            // restrict by _maxTopY (active area)
            if (newBoardPos.y >= _maxTopY)
                return false;

            // mushroom?
            if (cell.CellType == GameConstants.CellType.Mushroom)
                return false;

            // enemy?
            if (cell.CellType == GameConstants.CellType.Centipede || cell.CellType == GameConstants.CellType.Spider)
            {
                _isInjuredDuringMovement = true;
                return false;
            }

            SetBoardPosition(newBoardPos);

            _positionPointer = newPositionPointer;
            Follower.Follow(_positionPointer);
            return true;
        }

        #region BoardEntityBase
        public override GameConstants.CellType GetCellType()
        {
            return GameConstants.CellType.Player;
        }

        public override void Remove()
        {
            if (CurrentState == State.Dead)
                return;
            CurrentState = State.Dead;
            Visual.PlayDead();
            Board.CellAccessor.Set(_boardPosition, null);
            _deathTimer = DeathDuration;
        }
        #endregion
    }
}