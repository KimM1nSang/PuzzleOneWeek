using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TouchManager : Singleton<TouchManager>
{
    [HideInInspector] public Tile currentTile;
    [HideInInspector] public Tile prevTile;
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
            MouseUpTile();
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
        if (isTouch == false) return; // 마우스 안대고 있었으면 x

        // 이전 타일에 닿았으면 Undo
        if (prevTile == tile) // 바로 이전 타일이라면
        {
            if (CanUndo() == false) return;

            // 이전으로 돌아가기
            UndoTile();
            currentTile = tile;

            return;
        }

        // 추가하기 위한 조건 검사
        bool isNear = Vector3.Distance(currentTile.transform.position, tile.transform.position) <= 1f;
        bool isSame = currentTile.TileType == tile.TileType;
        bool isNotContain = tile.TileState != Define.TileState.SELECT;

        if (isNear && isSame && isNotContain)
        {
            prevTile = currentTile;
            AddLinkTile(tile); // 타일 추가
        }
    }

    private bool CanUndo()
    {
        return (linkTileList.Count >= 2) == true; // 2개 이상이면 Undo 가능
    }

    private Tile GetPrevTile() // 되돌아갈 타일을 반환
    {
        if (CanUndo() == false) // 되돌아갈 수 없다면(타일이 1개 이하) null
        {
            return null;
        }
        else
        {
            return linkTileList[linkTileList.Count - 2]; // 뒤에서 2번째 타일(=Prev) 반환
        }
    }

    private void AddLinkTile(Tile tile)
    {
        // Effect
        var linkEffect = Global.Pool.GetItem<TileLinkEffect>(); // 일단 이펙트 생성만 함
        linkEffect.transform.SetParent(tile.transform);
        linkEffect.transform.position = tile.transform.position;
        linkEffectList.Add(linkEffect);

        // Tile
        linkTileList.Add(tile);
        tile.TileState = Define.TileState.SELECT;
        currentTile = tile;

        // Line
        var linkTilePosList = from linkTile in linkTileList select linkTile.transform.position;
        touchingLine.positionCount = linkTilePosList.Count();
        touchingLine.SetPositions(linkTilePosList.ToArray());
    }

    public void MouseUpTile() // 타일에서 마우스를 뗄 떄
    {
        foreach (var effect in linkEffectList)
        {
            effect.gameObject.SetActive(false);
        }
        linkEffectList.Clear();

        GameManager.Instance.map.ClearSelectTile(linkTileList);

        // init
        isTouch = false;
        currentTile = null;
        firstTouchTile = null;
        prevTile = null;
        linkTileList.Clear();
        touchingLine.positionCount = 0;
    }

    public void UndoTile()
    {
        // Tile
        currentTile.TileState = Define.TileState.LIVE;
        linkTileList.Remove(currentTile);

        // Effect
        linkEffectList[linkEffectList.Count - 1].gameObject.SetActive(false);
        linkEffectList.RemoveAt(linkEffectList.Count - 1);

        // Line
        var linkTilePosList = from linkTile in linkTileList select linkTile.transform.position;
        touchingLine.positionCount = linkTilePosList.Count();
        touchingLine.SetPositions(linkTilePosList.ToArray());

        // Undo Tile Setting
        prevTile = GetPrevTile();
    }
}
