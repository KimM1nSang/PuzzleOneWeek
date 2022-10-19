using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TouchManager : Singleton<TouchManager>
{
    [HideInInspector] public Tile currentTouchTile;
    [HideInInspector] public Tile lastTouchTile;

    [SerializeField] GameObject linkEffect;
    [SerializeField] LineRenderer touchingLine;

    private bool isTouch;
    private List<Tile> linkTileList = new List<Tile>();
    private List<TileLinkEffect> linkEffectList = new List<TileLinkEffect>();

    //private Stack<Tile> linkTileStack = new Stack<Tile>();

    void Start()
    {
        Global.Pool.CreatePool<TileLinkEffect>(linkEffect, transform);
    }

    void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            DropTile();
        }
    }

    public void StartTile(Tile tile) // 처음 타일을 눌렀다
    {
        isTouch = true;
        AddLinkTile(tile);
    }

    public void EnterTile(Tile tile) // 타일 누름
    {
        if (isTouch == false) return;


        bool isNear = Vector3.Distance(currentTouchTile.transform.position, tile.transform.position) <= 1f;
        bool isSame = currentTouchTile.TileType == tile.TileType;
        bool isNotContain = tile.TileState != Define.TileState.SELECT;

        if (isNear && isSame)
        {
            if(lastTouchTile == tile)
            {
                // 이전으로 돌아가기
                currentTouchTile.TileState = Define.TileState.LIVE;
                linkTileList.Remove(currentTouchTile);
                RemoveLine();
                lastTouchTile = linkTileList[linkTileList.Count - 2];
                currentTouchTile = tile;

                return;
            }

            if (isNotContain)
            {
                lastTouchTile = currentTouchTile;
                AddLinkTile(tile);
            }
        }
    }

    private void AddLinkTile(Tile tile)
    {
        var linkEffect = Global.Pool.GetItem<TileLinkEffect>(); // 일단 이펙트 생성만 함
        linkEffect.transform.SetParent(tile.transform);
        linkEffect.transform.position = tile.transform.position;

        linkEffectList.Add(linkEffect);

        tile.TileState = Define.TileState.SELECT;
        AddLine(tile);

        currentTouchTile = tile;
    }

    public void DropTile()
    {
        foreach (var effect in linkEffectList)
        {
            effect.gameObject.SetActive(false);
        }
        linkEffectList.Clear();

        GameManager.Instance.map.ClearSelectTile(linkTileList);

        isTouch = false;
        currentTouchTile = null;
        linkTileList.Clear();
        touchingLine.positionCount = 0;
    }

    public void AddLine(Tile tile)
    {
        linkTileList.Add(tile);

        var linkTilePosList = from linkTile in linkTileList select linkTile.transform.position;

        touchingLine.positionCount = linkTilePosList.Count();
        touchingLine.SetPositions(linkTilePosList.ToArray());
    }

    public void RemoveLine()
    {
        linkEffectList.RemoveAt(linkEffectList.Count - 1);

        var linkTilePosList = from linkTile in linkTileList select linkTile.transform.position;

        touchingLine.positionCount = linkTilePosList.Count();
        touchingLine.SetPositions(linkTilePosList.ToArray());
    }
}
