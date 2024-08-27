using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Hexamap;

using Random = UnityEngine.Random;

[SelectionBase]
public abstract class TileBase : MonoBehaviour
{
    public bool canMove => canMoveLandform && structure == null && !isZombie;
    public bool isZombie { get; private set; }
    public bool isAccessable = true;
    public StructBase structure { get; private set; }
    public ZombieBase enemy { get; private set; }

    protected TileData tileData;

    private SpriteRenderer[] resourceIcons;
    private List<Resource> resources = new();
    private TileInfo info = new();

    bool canMoveLandform; private Vector3 _tileBounds = Vector3.zero;

    public Tile Model { get; private set; }
    public TileBorder Border { get; private set; }
    public Vector2 Coords => Model.Coords.ToVector();

    public void Initialize(Tile model, float padding)
    {
        Model = model;
        Model.Ctrl = this;
        Model.GameEntity = gameObject;
        transform.position = calculateWorldPosition(padding);
        name = $"{Model.Coords.ToString()} - {Model.Biome.Name} - {Model.Landform.GetType()}";

        if (GetTileType() != TileType.None)
        {
            Border = GetComponent<TileBorder>();

            var lanform = Model.Landform.GetType().Name;
            canMoveLandform = lanform == "LandformRocks" || lanform == "LandformPlain";

            info.img = Resources.Load<Sprite>("Illust/" + tileData.Code);
            info.landformTxt = tileData.Korean;
        }

    }

    private Vector3 calculateWorldPosition(float padding)
    {
        if (_tileBounds == Vector3.zero)
            _tileBounds = GetComponentInChildren<Renderer>().bounds.size;

        var tileSizeX = _tileBounds.x;
        var tileSizeY = _tileBounds.z;

        // Apply padding
        tileSizeX += tileSizeX * padding;
        tileSizeY += tileSizeY * padding;

        float x = Coords.x * tileSizeX / 2 * 1.5f;
        float y = Coords.y * tileSizeY;

        if (Coords.x % 2 == 0)
            y = Coords.y * tileSizeY + tileSizeY / 2;

        return new Vector3(x, 0, y);
    }

    public abstract TileType GetTileType();

    private void Awake()
    {
        if (App.Data.Game.tileData.TryGetValue(GetTileType().ToString(), out var data))
        {
            tileData = data;
        }

        resourceIcons = GetComponentsInChildren<SpriteRenderer>(true);
    }

    public void SetBuff()
    {
        Buff();
        DeBuff();
    }

    protected abstract void Buff();

    protected abstract void DeBuff();

    #region Set Resource
    public void SetResource()
    {
        var gachaProbability = SetTileData();
        var randomResources = gachaProbability
        .SelectMany(x => Enumerable.Repeat(x.Key, x.Value))
        .OrderBy(x => Guid.NewGuid())
        .Take(Random.Range(1, 3))
        .ToList();

        foreach (var resourceType in randomResources)
        {
            var code = "ITEM_" + resourceType.ToString().ToUpper();
            var resource = new Resource(code, Random.Range(1, 16));
            resources.Add(resource);
        }

        CheckAngle(transform.rotation.eulerAngles);
    }

    Dictionary<BasicItem, int> SetTileData()
    {
        resources.Clear();

        var gachaProbabilities = new Dictionary<BasicItem, int>
        {
            { BasicItem.Steel, tileData.RemainPossibility_Steel },
            { BasicItem.Carbon, tileData.RemainPossibility_Carbon },
            { BasicItem.Plasma, tileData.RemainPossibility_Plasma },
            { BasicItem.Powder, tileData.RemainPossibility_Powder },
            { BasicItem.Gas, tileData.RemainPossibility_Gas },
            { BasicItem.Rubber, tileData.RemainPossibility_Rubber }
        };

        return gachaProbabilities;
    }

    private void CheckAngle(Vector3 rotationValue)
    {
        if (resources.Count == 0) return;

        float rotationAngle = rotationValue.y + 90;

        switch (resources.Count)
        {
            case 1:
                resourceIcons[0].transform.Rotate(0, 0, rotationAngle);
                break;

            case 2:
                if (Mathf.Abs(rotationValue.y) == 0 || Mathf.Abs(rotationValue.y) == 180) //Rotate all icons if camera is vertical
                {
                    foreach (var icon in resourceIcons)
                    {
                        icon.transform.Rotate(0, 0, rotationAngle);
                    }
                }
                else // If camera is horizontal, rotate only second icon and rotate parent object
                {
                    resourceIcons[1].transform.parent.localEulerAngles = new Vector3(90, -rotationValue.y, 0);

                    foreach (var icon in resourceIcons)
                    {
                        icon.transform.Rotate(0, 0, 90);
                    }
                }
                break;
        }
    }

    public void SetSpecialResource(Resource _resource)
    {
        resources.Clear();

        resources.Add(_resource);

        CheckAngle(transform.rotation.eulerAngles);
    }
    #endregion

    #region Update Resource
    public void UpdateResource()
    {
        ResetTile();

        if (resources.Count == 0) return;

        switch (resources.Count)
        {
            case 1:
                SetIcon(resourceIcons[0], resources[0]);
                break;

            case 2:
                for (int i = 0; i < resources.Count; i++)
                    SetIcon(resourceIcons[i + 1], resources[i]);
                break;
        }
    }

    void ResetTile()
    {
        for (int i = 0; i < resources.Count; i++)
        {
            if (resources[i].Count == 0)
            {
                resources.RemoveAt(i);
                i--;
            }
        }

        foreach (var icon in resourceIcons)
        {
            icon.sprite = null;
            icon.gameObject.SetActive(false);
        }

        info.resourceTxt = "";
    }

    private void SetIcon(SpriteRenderer _icon, Resource _resource)
    {
        info.resourceTxt += _resource.Item.data.Korean + " " + _resource.Count + "EA\n";

        _icon.sprite = _resource.Item.itemImage;
        _icon.gameObject.SetActive(true);
    }
    #endregion

    #region Set Object
    public void SetStruct(StructBase _struct)
    {
        ResetTile();

        structure = _struct;

        info.landformTxt = _struct.name;
        info.resourceTxt = "자원 : ???";

        isAccessable = false;
    }

    public void SetEnemy(ZombieBase _enemy)
    {
        enemy = _enemy;

        info.enemyTxt = isZombie ? "좀비 수 : " + _enemy.count + "마리" : "";
    }
    #endregion

    public List<Resource> GetResources()
    {
        List<Resource> list = new();
        int resourceCount = App.Data.Test.Buff.resourceCount;

        foreach (var resource in resources)
        {
            var itemBase = resource.Item;

            list.Add(new Resource(itemBase, resourceCount));
            resource.Count = Mathf.Clamp(resource.Count - resourceCount, 0, int.MaxValue);
        }
        return list;
    }

    public void UpdateTileInfo()
    {
        App.Manager.UI.GetPanel<MapPanel>().SetInfo(info);
    }
}