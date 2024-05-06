using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using Hexamap;

public abstract class StructBase: MonoBehaviour
{
    protected StructData data;

    public Tile currTile;
    protected abstract string GetCode();

    public string name { get; protected set; }

    public Resource resource { get; protected set; }

    public bool isUse { get; protected set; }

    public bool isAccessible { get; protected set; }

    public List<Tile> neighborTiles { get; protected set; }

    public List<Tile> colleagues { get; protected set; }

    public List<TileBase> neighborBases => neighborTiles
            .Select(x => ((GameObject)x.GameEntity).GetComponent<TileBase>()).ToList();

    public List<TileBase> colleagueBases => colleagues
        .Select(x => ((GameObject)x.GameEntity).GetComponent<TileBase>()).ToList();

    public EStructure StructureType;

    public int visitDay { get; protected set; }

    public ItemData specialItem;

    private Material curMaterial;
    [SerializeField] private Renderer rend;
    [SerializeField] Material cloakingMaterial;

    public virtual void Init(List<Tile> _colleagueList)
    {
        data = App.Data.Game.structData[GetCode()];

        name = data.Korean;

        resource = new Resource(data.Item, data.Count);

        isUse = false;
        isAccessible = false;

        neighborTiles = _colleagueList.SelectMany(tile => tile.Neighbours.Values).Distinct().ToList();
        colleagues = _colleagueList;
        currTile = _colleagueList[0];
    }

    public virtual void Around()
    {
        App.Manager.UI.GetPanel<PagePanel>().SetSelectPage("structureSelect", this);
    }

    public abstract void YesFunc();
    public abstract void NoFunc();

    public void AllowAccess()
    {
        isUse = true;
        isAccessible = true;
    }

    void Start()
    {
        if (rend != null)
            curMaterial = rend.material;
    }

    public void FadeIn()
    {
        if (rend == null)
            return;

        rend.material = cloakingMaterial;
    }

    public void FadeOut()
    {
        if (rend == null)
            return;

        rend.material = curMaterial;
    }
}