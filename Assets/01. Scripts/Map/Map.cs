using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Map
{
    public Vector2 ScreenSize
    {
        get
        {
            return new Vector2(7, 6);
        }
    }

    Action _onEnemyMove;
    Action _onTreeMove;

    private List<Tile> gametiles = new List<Tile>();

    public void InitMap()
    {
        InitTile();
    }


    public void CeateGameScene()
    {

    }
    public void CreateGameAction()
    {

    }
    public void InitTile()
    {
        int coinXPos = 0;
        int coinYPos = 0;
        int diffX = 0/*104*/;
        int diffY = 0/*120*/;
        int initCoinXPos = 0/*72*/;
        int initCoinYPos = 0/*778*/;

        Tile gametile;

        // init array
        gametiles = new List<Tile>();

        // init tile
        for (int xIndex = 0; xIndex < ScreenSize.x; xIndex++)
        {
            coinXPos = initCoinXPos + (xIndex * diffX);
            coinYPos = initCoinYPos;
            if (xIndex % 2 == 0)
                coinYPos -= diffY / 2;

            for (int yIndex = 0; yIndex < ScreenSize.y; yIndex++)
            {
                gametile = CreateTile(new Vector2(coinXPos, coinYPos), Define.TileType.BLUE, Define.TileState.LIVE);

                gametiles.Add(gametile);

                coinYPos -= diffY;
            }
        }
    }
    public Tile CreateTile(Vector2 pos, Define.TileType type, Define.TileState state)
    {
        Tile tile = Global.Pool.GetItem<Tile>();
        tile.InitTile(state, type);
        tile.transform.position = pos;
        return tile;
    }
}
