using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private Define.TileState tileState;
    [property:SerializeField] public Define.TileState TileState {

        get
        {
            return tileState;
        }
        set
        {
            tileState = value;
            switch (tileState)
            {
                case Define.TileState.LIVE:
                    sr.color = Color.white;
                    break;
                case Define.TileState.DEAD:
                    gameObject.SetActive(false);
                    break;
                case Define.TileState.SELECT:
                    sr.color = Color.red;
                    break;
                case Define.TileState.MOVE:
                    break;
                default:
                    break;
            }
        }
    }

    [field:SerializeField] public Define.TileType TileType { get; set; }

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void InitTile(Define.TileState state, Define.TileType type)
    {
        this.TileState = state;
        this.TileType = type;
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        TouchManager.Instance.StartTile(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TouchManager.Instance.EnterTile(this);
        GameManager.Instance.map.GetTile(1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }
}
