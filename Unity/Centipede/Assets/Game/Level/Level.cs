using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using Utils;
using Random = UnityEngine.Random;

namespace Game
{
    public class Level : MonoBehaviour, IHandle<Spider.EventSpiderDied>
    {
        public BoardController Board;

        [Header("----- Prefabs")] public PlayerController PrefabPlayerController;
        public BoardEntityBase PrefabMushroom;
        public BoardEntityBase PrefabCentipede;
        public BoardEntityBase PrefabSpider;

        public BalanceConfig Balance { get; internal set; }
        public PlayerController Player { get; private set; }

        private float _nextSpiderCooldown;
        private float _nextCentipedCooldown;

        public void Start()
        {
            GlobalEventAggregator.EventAggregator.Subscribe(this);
            Assert.IsNotNull(Balance);
            SetTimersToFirstSpawnValues();
            StartCoroutine(CoroutineSpawnField());
        }

        public void Update()
        {
            if (StateGameplay.Instance.IsWin)
                return;

            // process enemies spawning by timer
            {
                _nextSpiderCooldown -= Time.deltaTime;
                _nextCentipedCooldown -= Time.deltaTime;
                if (_nextSpiderCooldown < 0f && !_isInCoroutineSpawnField)
                {
                    if (SpawnSpider())
                        _nextSpiderCooldown = float.PositiveInfinity; // controlled by EventSpiderDied
                }
                if (_nextCentipedCooldown < 0f && !_isInCoroutineSpawnField)
                {
                    _nextCentipedCooldown = float.PositiveInfinity;
                    SpawnCentipede();
                }
            }
        }

        public void RespawnOnlyEnemies()
        {
            // kill all entities
            Board.PurgeEnemies();

            // clear board (all except mushrooms)
            for (int x = 0; x < Board.CellAccessor.BoardSize.x; ++x)
                for (int y = 0; y < Board.CellAccessor.BoardSize.y; ++y)
                {
                    var cell = Board.CellAccessor.Get(x, y);
                    if (cell.CellType != GameConstants.CellType.Mushroom)
                        Board.CellAccessor.Set(x, y, null);
                }

            // reset spawning timers
            SetTimersToFirstSpawnValues();
        }

        private void SetTimersToFirstSpawnValues()
        {
            _nextCentipedCooldown = GameConstants.LevelStartCentipedeSpawninDelay;
            _nextSpiderCooldown = GameConstants.LevelStartSpiderSpawninDelay;
        }

        private bool _isInCoroutineSpawnField;
        IEnumerator CoroutineSpawnField()
        {
            _isInCoroutineSpawnField = true;
            // create field
            Board.SetGrid(Balance.GridSize);
            CameraGameplay.Instance.FocusOnBoardCenter();

            // spawn random mushrooms (but before first centipede appearing)
            var oneMushroomSpawnDelay = GameConstants.LevelStartCentipedeSpawninDelay / Balance.MushroomsAmount;
            for (int i = 0; i < Balance.MushroomsAmount; i++)
            {
                Vector2Int rndPosition = new Vector2Int(
                    (int)Mathf.Round((Balance.GridSize.x - 1) * Random.value),
                    (int)Mathf.Round((Balance.GridSize.y - 1) * Random.value));
                if (SpawnMushroom(rndPosition))
                    yield return new WaitForSeconds(oneMushroomSpawnDelay);
            }

            // create player
            yield return new WaitForSeconds(GameConstants.InitialSpawningDelay);
            SpawnPlayer();

            _isInCoroutineSpawnField = false;
            yield return null;
        }

        public void Handle(Spider.EventSpiderDied message)
        {
            _nextSpiderCooldown = Balance.NextSpiderTimeout;
        }

        #region Entities factory
        public bool SpawnMushroom(Vector2Int pos)
        {
            if (pos.y == 0 || pos.y == Balance.GridSize.y - 1) // don't spawn on bottom and top line
                return false;

            if (Board.CellAccessor.Get(pos).CellType != GameConstants.CellType.Empty)
                return false;

            var mushroom = Instantiate(PrefabMushroom, Board.TransformMushrooms) as Mushroom;
            Assert.IsNotNull(mushroom);
            mushroom.Init(Balance.MushroomsHitpoints, pos, Board);
            mushroom.name = PrefabMushroom.name;
            return true;
        }

        private void SpawnCentipede()
        {
            Vector2Int rndPosition = new Vector2Int(
                (int)Mathf.Round((Balance.GridSize.x - 1) * Random.value),
                Balance.GridSize.y - 1);
            var centipede = Instantiate(PrefabCentipede, Board.TransformEnemies) as Centipede;
            centipede.Init(
                Balance.CentipedeStepDelay,
                Balance.CentipedeSegmentsAmount,
                rndPosition,
                Board);
            centipede.SetActiveArea(Mathf.RoundToInt(Balance.GridSize.y * Balance.ActiveAreaOffsetPercent));
            centipede.name = PrefabCentipede.name;
        }

        public void SpawnCentipede(Centipede.Segment head, Centipede.Segment tail, Vector2Int moveDirection) // for split spawning
        {
            Assert.IsNotNull(head);
            var centipede = Instantiate(PrefabCentipede, Board.TransformEnemies) as Centipede;
            centipede.Init(
                Balance.CentipedeStepDelay, // todo: speed increase for splited cents
                head, tail, Board, moveDirection);
            centipede.SetActiveArea(Mathf.RoundToInt(Balance.GridSize.y * Balance.ActiveAreaOffsetPercent));
            centipede.name = PrefabCentipede.name;
        }

        public bool SpawnSpider()
        {
            Vector2Int spiderSpawnPosition = new Vector2Int(Balance.GridSize.x / 2, Balance.GridSize.y / 2);

            var destCell = Board.CellAccessor.Get(spiderSpawnPosition); // if something was spawned there
            if (destCell.CellType == GameConstants.CellType.Mushroom)
                destCell.Entity.Remove();

            if (destCell.CellType == GameConstants.CellType.Centipede)
                return false;

            var spider = Instantiate(PrefabSpider, Board.TransformEnemies) as Spider;
            spider.Init(Balance.SpiderStepDelay, spiderSpawnPosition, Board);
            spider.name = PrefabSpider.name;
            return true;
        }

        public void SpawnPlayer()
        {
            Vector2Int rndPosition = new Vector2Int((int)(Balance.GridSize.x * 0.5f), 0);
            var destCell = Board.CellAccessor.Get(rndPosition); // just in case if something was spawned there
            if (destCell.Entity != null)
                destCell.Entity.Remove();

            Player = Instantiate(PrefabPlayerController, Board.transform);
            Player.Init(rndPosition, Board, Balance);
            Player.name = PrefabPlayerController.name;
        }
        #endregion
    }
}