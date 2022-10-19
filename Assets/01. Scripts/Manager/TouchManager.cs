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

    public void StartTile(Tile tile) // ó�� Ÿ���� ������
    {
        isTouch = true;
        firstTouchTile = tile;
        AddLinkTile(tile);
    }

    public void EnterTile(Tile tile) // Ÿ�Ͽ� ���콺�� ��
    {
        if (isTouch == false) return;
        //if (tile == firstTouchTile) return;


        if (lastTouchTile == tile) // �ٷ� ���� Ÿ���̶��
        {
            if (linkTileList.Count <= 1) return;

            // �������� ���ư���
            currentTouchTile.TileState = Define.TileState.LIVE;
            linkTileList.Remove(currentTouchTile);
            RemoveLine();

            // ��� �̷��� ���ϰ� Stack ����Ǽ� ���߿� �����ϵ����ϰڽ��ϴ�..

            if (linkTileList.Count > 2) // 3�������� Ÿ��-����Ÿ��-����Ÿ���� �Ǵµ� 2������ ����Ÿ��-����Ÿ���϶� ����Ÿ���� ����� ����Ÿ���� ��� ������
            {
                lastTouchTile = linkTileList[linkTileList.Count - 2];
            }
            else // 2������ �ϳ� �������� ó�� Ŭ���� Ÿ�ϸ� ���Ҵٸ� ó�� Ÿ���� ������Ŭ���Ѱɷ� �ٲٱ�
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
        var linkEffect = Global.Pool.GetItem<TileLinkEffect>(); // �ϴ� ����Ʈ ������ ��
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
