using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TouchManager : Singleton<TouchManager>
{
    [HideInInspector] public Tile currentTouchTile; // ���� ��ȣ�ۿ� ���� Ÿ��
    //[HideInInspector] public Tile dragTile; // �巡���� Ÿ��

    public LineRenderer touchingLine;

    private bool isTouch;
    private List<Tile> linkTileList = new List<Tile>();

    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            DropTile();
        }
    }

    public void StartDragTile(Tile tile)
    {
        isTouch = true;
        currentTouchTile = tile;
        currentTouchTile.GetComponent<SpriteRenderer>().color = Color.red;
        AddLine(currentTouchTile);
    }

    public void DragTile(Tile tile) // Ÿ�� �ϳ� ������ �巡��������
    {
        if (isTouch == false) return;

        // currentTile�� �˻�

        bool isNear = Vector3.Distance(currentTouchTile.transform.position, tile.transform.position) <= 1f; // �Ÿ��� ���ٸ�
        bool isSame = currentTouchTile.TileType == tile.TileType; // ���� ���ٸ�
        bool isNotContain = linkTileList.Contains(tile) == false; // ���� �����Ű�� �ʾҴٸ�

        if (isNear && isSame && isNotContain) // �Ǵ³�
        {
            Debug.Log("Ÿ�� ����");
            
            currentTouchTile = tile;
            currentTouchTile.GetComponent<SpriteRenderer>().color = Color.red;
            AddLine(currentTouchTile);
        }
    }

    public void DropTile()
    {
        // ���� ��


        // �ʱ�ȭ
        isTouch = false;
        currentTouchTile = null;
        linkTileList.Clear();
        touchingLine.positionCount = 0;
    }

    public void AddLine(Tile tile)
    {
        linkTileList.Add(tile); // Ÿ���߰�

        var linkTilePosList = from linkTile in linkTileList select linkTile.transform.position; // ����� Ÿ�ϵ��� ��ġ

        touchingLine.positionCount = linkTilePosList.Count();
        touchingLine.SetPositions(linkTilePosList.ToArray()); // Ÿ�ϵ��� ��ġ�� ���� ���� ����
    }
}
