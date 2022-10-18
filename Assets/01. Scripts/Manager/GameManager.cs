using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Map map { get; } = new Map();

    public GameObject TilePrefab;
    public Transform TileParent;
    private void Start()
    {
        InitGame();
    }

    public void InitGame()
    {
        Global.Pool.CreatePool<Tile>(TilePrefab, TileParent, (int)(map.ScreenSize.x * map.ScreenSize.y));
        map.InitMap();
    }
}
