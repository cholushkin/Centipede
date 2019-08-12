using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Game
{
    public class Spider : BoardEntityBase
    {
        public class EventSpiderDied
        {
        }

        private static SimpleWaveProcessor<int> _waveProcessor;
        private static int[,] _map;
        private static readonly int Wall = 1;

        private float _speed;
        private Vector2Int _prevTargetPos = new Vector2Int(-1, -1);
        private Vector2Int _nextStep;
        private List<Vector2Int> _path;
        private float _currentStepCooldown;
        private float _attackinTimer;

        public void Init(float speed, Vector2Int boardPos, BoardController board)
        {
            Board = board;
            _speed = speed;
            _currentStepCooldown = _speed;
            SetBoardPosition(boardPos, false);
            transform.position = Board.ToWorldPosition(boardPos);
        }

        void Awake()
        {
            var currentMapSize = StateGameplay.Instance.Level.Balance.GridSize;
            _map = new int[currentMapSize.x, currentMapSize.y];
            _waveProcessor = new SimpleWaveProcessor<int>(_map, val => val == Wall);
        }

        void Update()
        {
            // move to target
            _currentStepCooldown -= Time.deltaTime;
            if (_currentStepCooldown < 0f)
            {
                _currentStepCooldown = _speed; // wait for next step
                SetBoardPosition(_nextStep);
                _nextStep = GetNextStep();
            }

            // get player
            var player = StateGameplay.Instance.Level.Player;
            if (player == null)
                return;

            // attacking the player
            var toPlayerDist = (player.GetBoardPosition() - _boardPosition).magnitude;
            if (Math.Abs(toPlayerDist - 1f) < 0.1f)
                _attackinTimer += Time.deltaTime;
            else
                _attackinTimer = 0f;
            if (_attackinTimer >= GameConstants.SpiderAttackingDuration)
                player.Remove();


            // calculate path
            if (player.GetBoardPosition() == _prevTargetPos)
                return;
            _prevTargetPos = player.GetBoardPosition();
            UpdateMap();
            _waveProcessor.Clear();
            _waveProcessor.SetTarget(_prevTargetPos);
            _waveProcessor.ComputeWaves(GetBoardPosition());
            _path = _waveProcessor.GetPath();
            GetNextStep(); // one node from the path
            _nextStep = GetNextStep();
        }

        private Vector2Int GetNextStep()
        {
            if (_path.Count < GameConstants.SpiderKeepDistanceRadius)
                return _nextStep;
            var step = _path[_path.Count - 1];
            _path.RemoveAt(_path.Count - 1);
            return step;
        }

        private void UpdateMap()
        {
            for (int x = 0; x < Board.CellAccessor.BoardSize.x; ++x)
            for (int y = 0; y < Board.CellAccessor.BoardSize.y; ++y)
            {
                _map[x, y] = 0;
                var cell = Board.CellAccessor.Get(x, y);
                if (cell.CellType == GameConstants.CellType.Mushroom ||
                    cell.CellType == GameConstants.CellType.Centipede)
                    _map[x, y] = Wall;
            }
        }

        public void ApplyDamage(float damage)
        {
            GlobalEventAggregator.EventAggregator.Publish(new EventSpiderDied());
            Remove();
        }

        public override GameConstants.CellType GetCellType()
        {
            return GameConstants.CellType.Spider;
        }

        public override void SetBoardPosition(Vector2Int pos, bool clearPrevPosition = true)
        {
            base.SetBoardPosition(pos, clearPrevPosition);
            transform.position = Board.ToWorldPosition(pos);
        }

        public override void Remove()
        {
            Board.CellAccessor.Set(_boardPosition, null);
            Destroy(gameObject);
        }
    }
}