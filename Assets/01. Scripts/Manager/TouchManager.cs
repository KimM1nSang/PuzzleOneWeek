using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TouchManager : Singleton<TouchManager>
{
    [HideInInspector] public Tile currentTouchTile;
    [HideInInspector] public Tile lastTouchTile;
    [HideInInspector] public Tile firstTouchTile;

    [SerializeField] GameObject linkEffect;
    [SerializeField] LineRenderer touchingLine;

    private bool isTouch;
    private List<Tile> linkTileList = new List<Tile>();
    private List<TileLinkEffect> linkEffectList = new List<TileLinkEffect>();

    private Stack<Tile> linkTileStack = new Stack<Tile>();

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
        firstTouchTile = tile;
        AddLinkTile(tile);
    }

    public void EnterTile(Tile tile) // 타일에 마우스를 댐
    {
        if (isTouch == false) return;
        //if (tile == firstTouchTile) return;


        if (lastTouchTile == tile) // 바로 이전 타일이라면
        {
            if (linkTileList.Count <= 1) return;

            // 이전으로 돌아가기
            currentTouchTile.TileState = Define.TileState.LIVE;
            linkTileList.Remove(currentTouchTile);
            RemoveLine();

            // 사실 이렇게 안하고 Stack 쓰면되서 나중에 수정하도록하겠습니다..

            if (linkTileList.Count > 2) // 3개까지는 타일-이전타일-현재타일이 되는데 2개에서 이전타일-현재타일일때 현재타일을 지우면 이전타일이 없어서 에러남
            {
                lastTouchTile = linkTileList[linkTileList.Count - 2];
            }
            else // 2개에서 하나 지워지고 처음 클릭한 타일만 남았다면 처음 타일을 마지막클릭한걸로 바꾸기
            {
                lastTouchTile = linkTileList[0];
            }
            currentTouchTile = tile;

            return;
        }

        bool isNear = Vector3.Distance(currentTouchTile.transform.position, tile.transform.position) <= 1f;
        bool isSame = currentTouchTile.TileType == tile.TileType;
        bool isNotContain = tile.TileState != Define.TileState.SELECT;

        if (isNear && isSame && isNotContain)
        {
                lastTouchTile = currentTouchTile;
                AddLinkTile(tile);
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
        firstTouchTile = null;
        lastTouchTile = null;
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
        linkEffectList[linkEffectList.Count - 1].gameObject.SetActive(false);
        linkEffectList.RemoveAt(linkEffectList.Count - 1);

        var linkTilePosList = from linkTile in linkTileList select linkTile.transform.position;

        touchingLine.positionCount = linkTilePosList.Count();
        touchingLine.SetPositions(linkTilePosList.ToArray());
    }
}
