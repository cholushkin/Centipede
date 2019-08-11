using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class Level : MonoBehaviour
{
    public BoardController Board;

    [Header("----- Prefabs")]
    public PlayerController PrefabPlayerController;
    public BoardEntityBase PrefabMushroom;
    public BoardEntityBase PrefabCentipede;

    public BalanceConfig Balance { get; internal set; }
    public PlayerController Player { get; private set; }

    private float _nextSpiderCooldown;
    private float _nextCentipedCooldown;

    public void Start()
    {
        Assert.IsNotNull(Balance);
        _nextCentipedCooldown = GameConstants.LevelStartCentipedeSpawninDelay; // initial animation delay
        StartCoroutine(CoroutineSpawnField());
    }
   
    public void Update()
    {
        // process enemies spawning
        _nextSpiderCooldown -= Time.deltaTime;
        _nextCentipedCooldown -= Time.deltaTime;
        if (_nextSpiderCooldown < 0f)
        {
            _nextSpiderCooldown = 777f; // todo: from balance
        }
        if (_nextCentipedCooldown < 0f)
        {
            _nextCentipedCooldown = 777f; // todo: from balance
            SpawnCentipede();
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
            if(cell.CellType != GameConstants.CellType.Mushroom)
                Board.CellAccessor.Set(x,y,null);
        }

        // reset spawning timers to zero
        _nextCentipedCooldown = 0f;
        _nextSpiderCooldown = Balance.NextSpiderTimeout * 0.5f;
    }

    IEnumerator CoroutineSpawnField()
    {
        // create field
        Board.SetGrid(Balance.GridSize);
        CameraGameplay.Instance.FocusOnBoardCenter();

        // spawn random mushrooms
        for (int i = 0; i < Balance.MushroomsAmount; i++)
        {
            Vector2Int rndPosition = new Vector2Int(
                (int)Mathf.Round((Balance.GridSize.x - 1) * Random.value),
                (int)Mathf.Round((Balance.GridSize.y - 1) * Random.value));
            if(SpawnMushroom(rndPosition))
                yield return new WaitForSeconds(GameConstants.InitialSpawningDelay);
        }

        // create player
        yield return new WaitForSeconds(GameConstants.InitialSpawningDelay);
        SpawnPlayer();

        yield return null;
    }
    #region Spawn entites
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
            (int) Mathf.Round((Balance.GridSize.x - 1) * Random.value),
            Balance.GridSize.y - 1);
        var centipede = Instantiate(PrefabCentipede, Board.TransformEnemies) as Centipede;
        centipede.Init(
            Balance.CentipedeSpeed,
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
            Balance.CentipedeSpeed, // todo: speed increase for splited cents
            head, tail, Board, moveDirection);
        centipede.SetActiveArea(Mathf.RoundToInt(Balance.GridSize.y * Balance.ActiveAreaOffsetPercent));
        centipede.name = PrefabCentipede.name;
    }

    public void SpawnSpider()
    {
        
    }

    public void SpawnPlayer()
    {
        Vector2Int rndPosition = new Vector2Int( (int) (Balance.GridSize.x * 0.5f), 0);
        var destCell = Board.CellAccessor.Get(rndPosition); // just in case if something was spawned there
        if( destCell.Entity != null)
            destCell.Entity.Remove();

        Player = Instantiate(PrefabPlayerController, Board.transform);
        Player.Init(
            rndPosition,
            Board, 
            Mathf.RoundToInt(Balance.GridSize.y * Balance.ActiveAreaOffsetPercent));
        Player.name = PrefabPlayerController.name;
    }
    #endregion

   
}
