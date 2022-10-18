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


    public void InitTile()
    {
        float coinXPos = 0;
        float coinYPos = 0;
        float diffX = .7f;
        float diffY = .7f;
        float initCoinXPos = -2.1f;
        float initCoinYPos = 1;

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
