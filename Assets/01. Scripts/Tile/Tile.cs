using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Define.TileState TileState { get; private set; }
    public Define.TileType TileType { get; private set; }

    public void InitTile(Define.TileState state, Define.TileType type)
    {
        this.TileState = state;
        this.TileType = type;
    }
}
