using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Define.TileState TileState { get; private set; }
    public Define.TileType TileType { get; private set; }

    public void InitTile(Define.TileState state, Define.TileType type)
    {
        this.TileState = state;
        this.TileType = type;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Ÿ�Ͽ� ���콺�� ��");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Ÿ�Ͽ��� ���콺�� ��");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Ÿ���� Ŭ����");
    }
}