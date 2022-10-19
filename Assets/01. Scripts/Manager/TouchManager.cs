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

    public void StartTile(Tile tile) // ó�� Ÿ���� ������
    {
        isTouch = true;
        firstTouchTile = tile;
        AddLinkTile(tile);
    }

    public void EnterTile(Tile tile) // Ÿ�Ͽ� ���콺�� ��
    {
        if (isTouch == false) return; // ���콺 �ȴ�� �־����� x

        // ���� Ÿ�Ͽ� ������� Undo
        if (prevTile == tile) // �ٷ� ���� Ÿ���̶��
        {
            if (CanUndo() == false) return;

            // �������� ���ư���
            UndoTile();
            currentTile = tile;

            return;
        }

        // �߰��ϱ� ���� ���� �˻�
        bool isNear = Vector3.Distance(currentTile.transform.position, tile.transform.position) <= 1f;
        bool isSame = currentTile.TileType == tile.TileType;
        bool isNotContain = tile.TileState != Define.TileState.SELECT;

        if (isNear && isSame && isNotContain)
        {
            prevTile = currentTile;
            AddLinkTile(tile); // Ÿ�� �߰�
        }
    }

    private bool CanUndo()
    {
        return (linkTileList.Count >= 2) == true; // 2�� �̻��̸� Undo ����
    }

    private Tile GetPrevTile() // �ǵ��ư� Ÿ���� ��ȯ
    {
        if (CanUndo() == false) // �ǵ��ư� �� ���ٸ�(Ÿ���� 1�� ����) null
        {
            return null;
        }
        else
        {
            return linkTileList[linkTileList.Count - 2]; // �ڿ��� 2��° Ÿ��(=Prev) ��ȯ
        }
    }

    private void AddLinkTile(Tile tile)
    {
        // Effect
        var linkEffect = Global.Pool.GetItem<TileLinkEffect>(); // �ϴ� ����Ʈ ������ ��
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

    public void MouseUpTile() // Ÿ�Ͽ��� ���콺�� �� ��
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
