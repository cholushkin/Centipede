using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class Level : MonoBehaviour
{
    public BoardController Board;

    [Header("----- Prefabs")]
    public PlayerController PrefabPlayerController;
    public BoardEntityBase PrefabMushroom;

    public BalanceConfig Balance { get; internal set; }
    public PlayerController Player { get; private set; }

    public void Start()
    {
        Assert.IsNotNull(Balance);
        StartCoroutine(CoroutineSpawnField());
    }

    public void SpawnPlayer()
    {
        Player = Instantiate(PrefabPlayerController, transform);
        Player.name = PrefabPlayerController.name;
        Player.Board = Board;
        Player.Weapon.Board = Board;
    }

    public void Update()
    {
        // spawn worm
        // spawn spider
    }

    IEnumerator CoroutineSpawnField()
    {
        // create field
        Board.SetGrid(Balance.GridSize);
        CameraGameplay.Instance.FocusOnBoardCenter();


        // spawn mushrooms
        for (int i = 0; i < Balance.MushroomsAmount; i++)
        {
            // set mushroom
            Vector2Int rndPosition = new Vector2Int(
                (int)Mathf.Round((Balance.GridSize.x - 1) * Random.value),
                (int)Mathf.Round((Balance.GridSize.y - 1) * Random.value));
            rndPosition.y = Mathf.Clamp(rndPosition.y, 2, Balance.GridSize.y - 1); // don't spawn on bottom 2 lines

            if (Board.CellAccessor.Get(rndPosition).CellType != GameConstants.CellType.Empty)
                continue;

            yield return new WaitForSeconds(GameConstants.InitialSpawningDelay);
            var mushroom = Instantiate(PrefabMushroom, transform);
            mushroom.Board = Board;
            mushroom.name = string.Format("Mushroom.{0}", i);
            mushroom.SetPosition(rndPosition, false);
            mushroom.transform.position = Board.ToWorldPosition(rndPosition);
        }

        // create player
        yield return new WaitForSeconds(GameConstants.InitialSpawningDelay);
        SpawnPlayer();
        Player.SetActiveArea(Mathf.RoundToInt(Balance.GridSize.y * Balance.ActiveAreaOffsetPercent));

        yield return null;
    }
}
