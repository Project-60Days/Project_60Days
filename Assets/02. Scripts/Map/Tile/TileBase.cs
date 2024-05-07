using System;
using System.Collections.Generic;
using UnityEngine;
using Hexamap;
using System.Linq;
using Random = UnityEngine.Random;

[SelectionBase]
public abstract class TileBase : MonoBehaviour
{
    protected int resourceCount = 2;
    public abstract TileType GetTileType();

    [SerializeField] Sprite landformSprite;
    [SerializeField] SpriteRenderer[] resourceIcons;

    Dictionary<EResourceType, int> gachaProbability;
    List<EResourceType> gachaList;
    List<Resource> appearanceResources;

    Tile tile;
    public TileData tileData { get; private set; }

    ItemSO itemSO;


    string resourceText;
    string landformText;

    public ZombieBase currZombies { get; private set; }

    public StructBase structure { get; private set; }

    protected virtual void Awake()
    {
        gachaProbability = new Dictionary<EResourceType, int>();
        gachaList = new List<EResourceType>();
        appearanceResources = new List<Resource>();

        App.Data.Game.tileData.TryGetValue(GetTileType().ToString(), out TileData data);
        tileData = data;
        landformText = tileData.Korean;
    }

    void Start()
    {
        Player.PlayerSightUpdate += CheckPlayerTIle;
        tile = GetComponent<TileController>().Model;

        itemSO = App.Manager.Game.itemSO;

        var random = Random.Range(0, 100);
        if (random < App.Manager.Test.mapData.resourcePercent)
            SpawnRandomResource();
    }

    void OnDestroy()
    {
        Player.PlayerSightUpdate -= CheckPlayerTIle;
    }

    public abstract void Buff();

    public abstract void DeBuff();

    void GetTilData()
    {
        gachaList.Clear();
        appearanceResources.Clear();
        gachaProbability.Clear();

        gachaProbability.Add(EResourceType.Steel, tileData.RemainPossibility_Steel);
        gachaProbability.Add(EResourceType.Carbon, tileData.RemainPossibility_Carbon);
        gachaProbability.Add(EResourceType.Plasma, tileData.RemainPossibility_Plasma);
        gachaProbability.Add(EResourceType.Powder, tileData.RemainPossibility_Powder);
        gachaProbability.Add(EResourceType.Gas, tileData.RemainPossibility_Gas);
        gachaProbability.Add(EResourceType.Rubber, tileData.RemainPossibility_Rubber);
    }

    public void SpawnRandomResource()
    {
        GetTilData();

        var randomInt = Random.Range(1, 3);

        while (gachaList.Count != randomInt)
        {
            var take = WeightedRandomizer.From(gachaProbability).TakeOne();
            if (gachaList.Contains(take) == false)
                gachaList.Add(take);
        }

        for (int i = 0; i < gachaList.Count; i++)
        {
            var code = "ITEM_" + gachaList[i].ToString().ToUpper();
            var resource = new Resource(code, Random.Range(1, 16));
            appearanceResources.Add(resource);
        }

        RotationCheck(transform.rotation.eulerAngles);
    }

    public void ResourceUpdate(bool _isInPlayerSight)
    {
        if (structure != null)
        {
            if (structure.isAccessible == false)
                return;
        }

        if (_isInPlayerSight)
        {
            ResourceInit();

            if (appearanceResources.Count > 0)
            {
                var text = "";

                for (int i = 0; i < appearanceResources.Count; i++)
                {
                    text += appearanceResources[i].Item.data.Korean + " " +
                            appearanceResources[i].Count + "EA\n";
                }

                resourceText = text;

                if (appearanceResources.Count == 1)
                {
                    resourceIcons[0].sprite = appearanceResources[0].Item.itemImage;
                    resourceIcons[0].gameObject.SetActive(true);
                }
                else if (appearanceResources.Count == 2)
                {
                    for (int i = 0; i < appearanceResources.Count; i++)
                    {
                        SpriteRenderer item = resourceIcons[i + 1];
                        item.sprite = appearanceResources[i].Item.itemImage;
                        item.gameObject.SetActive(true);
                    }
                }
                else
                {
                    for (int i = 0; i < appearanceResources.Count; i++)
                    {
                        SpriteRenderer item = resourceIcons[i + 3];
                        item.sprite = appearanceResources[i].Item.itemImage;
                        item.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                resourceText = "자원 없음";

                for (int i = 0; i < resourceIcons.Length; i++)
                {
                    SpriteRenderer item = resourceIcons[i];
                    item.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            resourceText = "자원 : ???";

            for (int i = 0; i < resourceIcons.Length; i++)
            {
                SpriteRenderer item = resourceIcons[i];
                item.gameObject.SetActive(false);
            }
        }
    }

    void ResourceInit()
    {
        for (int i = 0; i < appearanceResources.Count; i++)
        {
            Resource item = appearanceResources[i];

            if (item.Count == 0)
            {
                appearanceResources.Remove(item);
            }
        }

        for (int i = 0; i < resourceIcons.Length; i++)
        {
            SpriteRenderer icon = resourceIcons[i];
            icon.sprite = null;
            icon.gameObject.SetActive(false);
        }
    }


    protected void RotationCheck(Vector3 rotationValue)
    {
        if (appearanceResources.Count == 1)
        {
            SpriteRenderer item = resourceIcons[0];
            item.gameObject.transform.Rotate(0, 0, rotationValue.y + 90);
        }
        else if (appearanceResources.Count == 2)
        {
            if (Mathf.Abs(rotationValue.y) == 0 || Mathf.Abs(rotationValue.y) == 180)
            {
                for (int i = 0; i < resourceIcons.Length; i++)
                {
                    SpriteRenderer item = resourceIcons[i];
                    item.gameObject.transform.Rotate(0, 0, rotationValue.y + 90);
                }
            }
            else
            {
                resourceIcons[1].transform.parent.transform.localEulerAngles =
                    new Vector3(90, -rotationValue.y, 0);

                for (int i = 0; i < resourceIcons.Length; i++)
                {
                    SpriteRenderer item = resourceIcons[i];
                    item.gameObject.transform.Rotate(0, 0, 90);
                }
            }
        }
    }

    public List<Resource> GetResources()
    {
        List<Resource> list = new List<Resource>();

        if (appearanceResources == null) return null;

        foreach (var resource in appearanceResources)
        {
            var itemBase = resource.Item;

            list.Add(new Resource(itemBase, resourceCount));
            resource.Count = Clamp(resource.Count - resourceCount);
        }

        ResourceUpdate(true);
        return list;
    }

    private int Clamp(int value) => value < 0 ? 0 : value;

    public bool CheckResources()
    {
        if (appearanceResources != null)
            return true;
        else
            return false;
    }

    void CheckPlayerTIle()
    {
        bool check = App.Manager.Map.GetTilesInRange(2).Contains(tile);
        ResourceUpdate(check);
    }

    public void TileInfoUpdate()
    {
        App.Manager.UI.GetPanel<MapPanel>().UpdateImage(landformSprite);
        App.Manager.UI.GetPanel<MapPanel>().UpdateText(TileInfo.Landform, landformText);
        App.Manager.UI.GetPanel<MapPanel>().UpdateText(TileInfo.Resource, resourceText);

        if (currZombies == null)
            App.Manager.UI.GetPanel<MapPanel>().UpdateText(TileInfo.Zombie, "좀비 수 : ???");
        else
        {
            App.Manager.UI.GetPanel<MapPanel>()
                .UpdateText(TileInfo.Zombie, "좀비 수 : " + currZombies.count + "마리");
        }
    }

    public void SetTower(StructBase _struct)
    {
        SetItemNull();

        structure = _struct;

        landformText = "생산 공장";
        resourceText = "자원 : ???";

        TileInfoUpdate();
    }

    public void SetProduction(StructBase _struct)
    {
        SetItemNull();

        structure = _struct;

        landformText = "생산 공장";
        resourceText = "자원 : ???";
    }

    public void SetArmy(StructBase _struct)
    {
        SetItemNull();

        structure = _struct;

        landformText = "군사 기지";
        resourceText = "자원 : ???";
    }

    private void SetItemNull()
    {
        for (int i = 0; i < resourceIcons.Length; i++)
        {
            SpriteRenderer item = resourceIcons[i];
            item.sprite = null;
            item.gameObject.SetActive(false);
        }
    }

    public void AddSpecialItem()
    {
        ItemBase itemBase;

        if (appearanceResources.Count == 2)
            appearanceResources.RemoveRange(appearanceResources.Count - 1, 1);

        if (structure.specialItem != null)
        {
            itemBase = itemSO.items.ToList()
                .Find(x => x.data == structure.specialItem);
        }
        else
        {
            itemBase = itemSO.items.ToList()
                .Find(x => x.data == structure.resource.Item.data);
        }

        // 특수 자원 추가
        appearanceResources.Add(new Resource(itemBase, 1));

        RotationCheck(transform.rotation.eulerAngles);
        ResourceUpdate(true);
    }

    public void UpdateZombieInfo(ZombieBase zombie)
    {
        if (zombie == null)
        {
            currZombies = null;
            App.Manager.UI.GetPanel<MapPanel>().UpdateText(TileInfo.Zombie, "좀비 수 : ???");
        }
        else
        {
            currZombies = zombie;
            App.Manager.UI.GetPanel<MapPanel>()
                .UpdateText(TileInfo.Zombie, "좀비 수 : " + currZombies.count + "마리");
        }
    }
}