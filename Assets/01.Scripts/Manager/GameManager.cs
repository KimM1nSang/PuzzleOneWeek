using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Map map { get; }

    public GameObject tilePrefab;
    private void Start()
    {
    }

    public void InitGame()
    {
        Global.Pool.CreatePool<Tile>(tilePrefab,transform,(int)(map.ScreenSize.x * map.ScreenSize.y));
        map.InitMap();
    }
}
