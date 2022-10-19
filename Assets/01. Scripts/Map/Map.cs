using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
public class Map
{
    public Vector2 MapSize
    {
        get
        {
            return new Vector2(7, 6);
        }
    }

    private float diffX = .7f;
    private float diffY = .7f;
    private float initCoinXPos = -2.1f;
    private float initCoinYPos = 1;

    Action OnTileMoveEnd;

    private List<Tile> gametiles = new List<Tile>();
    private List<int> lineDeadTiles = new List<int>();
    public void InitMap()
    {
        InitTile();
    }


    public void InitTile()
    {
        float coinXPos = 0;
        float coinYPos = 0;
    

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
        if (tiles.Count >= 3)
        {
            foreach (var item in tiles)
            {
                item.InitTile(Define.TileState.DEAD,item.TileType);
                item.gameObject.SetActive(false);
            }
        }
        else
        {
            foreach (var item in tiles)
            {
                item.InitTile(Define.TileState.LIVE,item.TileType);
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
                {
                    ChangeTile(y, y + deadTileNum);
                }
            }
        }
        AddTile();
    }
    public void AddTile()
    {
        Tile tile;
        int line = 0;
        lineDeadTiles.Clear();

        for (int i = 0; i < MapSize.x; i++)
            lineDeadTiles.Add(0);

        for (int i = 0; i < gametiles.Count; i++)
        {
            tile = gametiles[i];

            if (tile.TileState ==Define.TileState.DEAD)
            {
                line = (int)(i / MapSize.y);
                lineDeadTiles[line] += 1;

                SetNewTile(tile);
            }
        }
        AddNewTileAction();
    }
    public void SetNewTile(Tile tile)
    {
        tile.gameObject.SetActive(true);
        tile.InitTile(Define.TileState.LIVE,Define.TileType.BLUE);
    }
    public void AddNewTileAction()
    {
        Tile tile;
        Vector2 pos;
        int startIndex;

        for (int i = 0; i < MapSize.x; i++)
        {
            if (lineDeadTiles[i] > 0)
            {
                startIndex = i * (int)MapSize.y;

                for (int j = startIndex; j < startIndex + lineDeadTiles[i]; j++)
                {
                    tile = gametiles[j];
                    pos = tile.transform.position;
                    tile.transform.position = new Vector2(pos.x, pos.y + (lineDeadTiles[i] * diffY));

                    MoveTile(tile, pos);
                }
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
        tile2.transform.position = tile1.transform.position ;
        /* 
            tile1.transform.position = tempPos;
    */
        MoveTile(tile1,tempPos);
        tempTile = tile2;
        gametiles[idx2] = tile1;
        gametiles[idx1] = tempTile;
        
    }
    private void MoveTile(Tile tile,Vector2 pos)
    {
        float duration = .5f;
        tile.InitTile(Define.TileState.MOVE, tile.TileType);
        DOTween.Kill(tile);
        tile.transform.DOMove(pos,duration).SetDelay(.5f).OnComplete(()=> {
            tile.InitTile(Define.TileState.LIVE, tile.TileType);
            OnTileMoveEnd?.Invoke();
        });
    }

    public Tile CreateTile(Vector2 pos, Define.TileType type, Define.TileState state)
    {
        Tile tile = Global.Pool.GetItem<Tile>();
        tile.InitTile(state, type);
        tile.transform.position = pos;
        return tile;
    }

    public Tile GetTile(int index)
    {
        return gametiles[index];
    }
}
