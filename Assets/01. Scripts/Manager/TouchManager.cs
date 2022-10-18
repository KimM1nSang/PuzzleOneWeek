using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TouchManager : Singleton<TouchManager>
{
    [HideInInspector] public Tile currentTouchTile; // 현재 상호작용 중인 타일
    //[HideInInspector] public Tile dragTile; // 드래그한 타일

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

    public void DragTile(Tile tile) // 타일 하나 누른후 드래그했을때
    {
        if (isTouch == false) return;

        // currentTile과 검사

        bool isNear = Vector3.Distance(currentTouchTile.transform.position, tile.transform.position) <= 1f; // 거리가 좁다면
        bool isSame = currentTouchTile.TileType == tile.TileType; // 색이 같다면
        bool isNotContain = linkTileList.Contains(tile) == false; // 아직 연결시키지 않았다면

        if (isNear && isSame && isNotContain) // 되는놈
        {
            Debug.Log("타일 연결");
            
            currentTouchTile = tile;
            currentTouchTile.GetComponent<SpriteRenderer>().color = Color.red;
            AddLine(currentTouchTile);
        }
    }

    public void DropTile()
    {
        // 삭제 빔


        // 초기화
        isTouch = false;
        currentTouchTile = null;
        linkTileList.Clear();
        touchingLine.positionCount = 0;
    }

    public void AddLine(Tile tile)
    {
        linkTileList.Add(tile); // 타일추가

        var linkTilePosList = from linkTile in linkTileList select linkTile.transform.position; // 연결된 타일들의 위치

        touchingLine.positionCount = linkTilePosList.Count();
        touchingLine.SetPositions(linkTilePosList.ToArray()); // 타일들의 위치를 통해 라인 생성
    }
}
