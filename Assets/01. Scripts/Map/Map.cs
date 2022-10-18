using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Map
{
    public Vector2 MapSize
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
        for (int xIndex = 0; xIndex < MapSize.x; xIndex++)
        {
            coinXPos = initCoinXPos + (xIndex * diffX);
            coinYPos = initCoinYPos;
            if (xIndex % 2 == 0)
                coinYPos -= diffY / 2;

            for (int yIndex = 0; yIndex < MapSize.y; yIndex++)
            {
                gametile = CreateTile(new Vector2(coinXPos, coinYPos), Define.TileType.BLUE, Define.TileState.LIVE);

                gametiles.Add(gametile);

                coinYPos -= diffY;
            }
        }
    }
    public void ClearSelectTile(List<Tile> tiles)
    {
        if (tiles.Count > 3)
        {
            foreach (var item in tiles)
            {
                item.InitTile(Define.TileState.DEAD,item.TileType);
            }
        }
        MoveUpDeadTile();
    }
    public void MoveUpDeadTile()
    {
        Tile tile;
        int deadTileNum = 0;
        int bottomPos = 0;

        for (int x = 0; x < MapSize.x; x++)
        {
            bottomPos = (int)(((x + 1) * MapSize.y) - 1);
            deadTileNum = 0;

            for (int y = bottomPos; y > bottomPos - MapSize.y; y--)
            {
                tile = gametiles[y];
                if (tile.TileState == Define.TileState.DEAD)
                {
                    deadTileNum++;
                    continue;
                }

                if (deadTileNum > 0)
                    ChangeTile(y, y + deadTileNum);
            }
        }
    }
    private void ChangeTile(int idx1,int idx2)
    {
        Vector2 tempPos;
        Tile tempTile;
        Tile tile1 = gametiles[idx1];
        Tile tile2 = gametiles[idx2];
        tempPos = tile2.transform.position;
        tile2.transform.position = tile1.transform.position;
        tile1.transform.position = tempPos;

        tempTile = tile2;
        gametiles[idx2] = tile1;
        gametiles[idx1] = tempTile;

    }

    public Tile CreateTile(Vector2 pos, Define.TileType type, Define.TileState state)
    {
        Tile tile = Global.Pool.GetItem<Tile>();
        tile.InitTile(state, type);
        tile.transform.position = pos;
        return tile;
    }
}
