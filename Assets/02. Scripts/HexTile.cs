using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTile : MonoBehaviour
{
    public HexTile[] neighbours; //인접한 헥사타일 객체 배열

    public enum TerrainType{ Grassland, Machine, Desert, Water }; //타일 타입
    public enum TileHeight{ Flat, Uphill, Downhill }; //지형 타입

    protected TerrainType terrainType;
    protected TileHeight tileHeight;
    protected bool moveAble; //이동 가능 여부
    protected GameObject deck; //타일 위의 장식 또는 구조물

    public virtual bool IsPassable()
    {
        return moveAble;
    }

    protected Color defaultColor; //기본 색상
    protected Color selectedColor = Color.yellow; //선택됐을 때 색상

    void Start()
    {
        defaultColor = GetComponent<Renderer>().material.color;
    }
    public virtual void OnTileClicked() //타일이 선택됐을 때
    {
        GetComponent<Renderer>().material.color = selectedColor;
    }
}
