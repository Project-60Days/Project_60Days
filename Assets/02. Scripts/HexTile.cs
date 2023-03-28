using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTile : MonoBehaviour
{
    public HexTile[] neighbours; //인접한 헥사타일 객체 배열

    protected TerrainType terrainType;
    protected TileHeight tileHeight;
    protected bool moveAble; //이동 가능 여부
    protected GameObject deck; //타일 위의 장식 또는 구조물
    protected Color defaultColor; //기본 색상
    protected Color selectedColor = Color.yellow; //선택됐을 때 색상

    void Start()
    {
        defaultColor = GetComponent<Renderer>().material.color;
    }

    /// <summary>
    /// 이동 가능 여부 반환
    /// </summary>
    public virtual bool IsPassable()
    {
        return moveAble;
    }

    /// <summary>
    /// 타일이 선택됐을 때 타일 색상 변경
    /// </summary>
    public virtual void OnTileClicked()
    {
        GetComponent<Renderer>().material.color = selectedColor;
    }
}
