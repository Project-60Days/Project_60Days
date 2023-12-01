using System.Collections.Generic;
using UnityEngine;
using Hexamap;
using System.Linq;
using Random = UnityEngine.Random;

[SelectionBase]
public class TileBase : MonoBehaviour
{
    #region Variables

    [SerializeField] ItemSO itemSO;
    [SerializeField] ETileType tileType;
    [SerializeField] Sprite landformSprite;
    [SerializeField] SpriteRenderer[] resourceIcons;

    Dictionary<EResourceType, int> gachaProbability;
    List<EResourceType> gachaList;
    List<Resource> appearanceResources;

    Tile tile;
    public Tile Tile => tile;

    TileData tileData;
    public TileData TileData => tileData;

    bool inPlayerSight;
    bool isStructureNeighbor;

    public bool IsStructureNeighbor => isStructureNeighbor;

    string resourceText;
    string landformText;
    
    ZombieBase curZombies;

    public ZombieBase CurZombies => curZombies;

    StructureBase structure;

    public StructureBase Structure => structure;

    #endregion

    void Start()
    {
        Player.PlayerSightUpdate += CheckPlayerTIle;
        Init();
    }

    void OnDestroy()
    {
        Player.PlayerSightUpdate -= CheckPlayerTIle;
    }

    public void Init()
    {
        gachaProbability = new Dictionary<EResourceType, int>();
        gachaList = new List<EResourceType>();
        appearanceResources = new List<Resource>();
        tile = gameObject.transform.GetComponent<TileController>().Model;

        App.instance.GetDataManager().tileData.TryGetValue(GetTileDataIndex(), out TileData data);
        tileData = data;
        landformText = tileData.Korean;

        gachaProbability.Add(EResourceType.Steel, tileData.RemainPossibility_Steel);
        gachaProbability.Add(EResourceType.Carbon, tileData.RemainPossibility_Carbon);
        gachaProbability.Add(EResourceType.Plasma, tileData.RemainPossibility_Plasma);
        gachaProbability.Add(EResourceType.Powder, tileData.RemainPossibility_Powder);
        gachaProbability.Add(EResourceType.Gas, tileData.RemainPossibility_Gas);
        gachaProbability.Add(EResourceType.Rubber, tileData.RemainPossibility_Rubber);
        
        SpawnRandomResource();
        RotationCheck(transform.rotation.eulerAngles);
    }

    void SpawnRandomResource()
    {
        var randomInt = Random.Range(1, 3);
        while (gachaList.Count != randomInt)
        {
            var take = WeightedRandomizer.From(gachaProbability).TakeOne();
            if (gachaList.Contains(take) == false)
                gachaList.Add(take);
        }

        for (int i = 0; i < gachaList.Count; i++)
        {
            var itemBase = itemSO.items.ToList()
                .Find(x => x.data.English == gachaList[i].ToString());

            var resource = new Resource(gachaList[i].ToString(), Random.Range(1, 16), itemBase);
            appearanceResources.Add(resource);
        }
    }

    void ResourceUpdate(bool _isInPlayerSight)
    {
        if (structure != null)
            return;

        if (_isInPlayerSight == true)
        {
            for (int i = 0; i < appearanceResources.Count; i++)
            {
                Resource item = appearanceResources[i];
                if (item.ItemCount == 0)
                {
                    appearanceResources.Remove(item);
                }
            }

            for (int i = 0; i < resourceIcons.Length; i++)
            {
                SpriteRenderer item = resourceIcons[i];
                item.sprite = null;
                item.gameObject.SetActive(false);
            }

            if (appearanceResources.Count > 0)
            {
                bool isItem = appearanceResources.Count > 1 ? true : false;
                for (int i = 0; i < appearanceResources.Count; i++)
                {
                    SpriteRenderer itemIcon;
                    if (isItem == true)
                        itemIcon = resourceIcons[i + 1];
                    else
                    {
                        itemIcon = resourceIcons[i];
                    }

                    itemIcon.sprite = appearanceResources[i].ItemBase.itemImage;
                    itemIcon.gameObject.SetActive(true);
                }

                if (appearanceResources.Count == 1)
                {
                    resourceText = appearanceResources[0].ItemBase.data.Korean + " " +
                                   appearanceResources[0].ItemCount + "EA\n";
                }
                else
                {
                    resourceText = appearanceResources[0].ItemBase.data.Korean + " " +
                                   appearanceResources[0].ItemCount + "EA\n" +
                                   appearanceResources[1].ItemBase.data.Korean + " " +
                                   appearanceResources[1].ItemCount + "EA\n";
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

    protected void RotationCheck(Vector3 rotationValue)
    {
        if (appearanceResources.Count == 1)
        {
            for (int i = 0; i < resourceIcons.Length; i++)
            {
                SpriteRenderer item = resourceIcons[i];
                item.gameObject.transform.Rotate(0, 0, rotationValue.y + 90);
            }
        }
        else
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
                resourceIcons[resourceIcons.Length - 1].transform.parent.transform.localEulerAngles =
                    new Vector3(90, -rotationValue.y, 0);

                for (int i = 0; i < resourceIcons.Length; i++)
                {
                    SpriteRenderer item = resourceIcons[i];
                    item.gameObject.transform.Rotate(0, 0, 90);
                }
            }
        }
    }

    public List<Resource> GetResources(int count)
    {
        List<Resource> list = new List<Resource>();

        if (appearanceResources == null)
        {
            Debug.Log("자원이 없습니다.");
            return null;
        }

        for (int i = 0; i < appearanceResources.Count; i++)
        {
            Resource item = appearanceResources[i];
            var itemBase = itemSO.items.ToList()
                .Find(x => x.data.English == appearanceResources[i].ItemCode);
            if (item.ItemCount - count >= 0)
            {
                list.Add(new Resource(item.ItemCode, count, itemBase));
                item.ItemCount -= count;
            }
            else
            {
                list.Add(new Resource(item.ItemCode, item.ItemCount, itemBase));
                item.ItemCount -= count;
            }
        }

        ResourceUpdate(true);
        return list;
    }

    public bool CheckResources()
    {
        if (appearanceResources != null)
            return true;
        else
            return false;
    }

    void CheckPlayerTIle(Tile tile)
    {
        if (App.instance.GetMapManager().mapController.GetTilesInRange(tile, 3).Contains(this.tile) ||
            this.tile == tile)
        {
            ResourceUpdate(true);
        }
        else
        {
            ResourceUpdate(false);
        }
    }

    public void TileInfoUpdate()
    {
        App.instance.GetMapManager().mapUIController.UpdateImage(landformSprite);
        App.instance.GetMapManager().mapUIController.UpdateText(ETileInfoTMP.Landform, landformText);
        App.instance.GetMapManager().mapUIController.UpdateText(ETileInfoTMP.Resource, resourceText);
        
        if(curZombies == null)
            App.instance.GetMapManager().mapUIController.UpdateText(ETileInfoTMP.Zombie, "좀비 수 : ???");
        else
        {
            App.instance.GetMapManager().mapUIController.UpdateText(ETileInfoTMP.Zombie, "좀비 수 : " + curZombies.zombieData.count + "마리");
        }
    }

    public void SpawnTower(List<Tile> neighborTiles)
    {
        var neighborBases = neighborTiles
            .Select(x => ((GameObject)x.GameEntity).GetComponent<TileBase>()).ToList();

        structure = new Tower();
        ((Tower)structure).Init(neighborBases);

        landformText = "타워";
        resourceText = "자원 : ???";
        
        for (int i = 0; i < resourceIcons.Length; i++)
        {
            SpriteRenderer item = resourceIcons[i];
            item.sprite = null;
            item.gameObject.SetActive(false);
        }
    }

    public void SpawnNormalStructure(List<Tile> neighborTiles, List<Tile> colleagueTiles)
    {
        var neighborBases = neighborTiles
            .Select(x => ((GameObject)x.GameEntity).GetComponent<TileBase>()).ToList();

        var colleagueBases = colleagueTiles
            .Select(x => ((GameObject)x.GameEntity).GetComponent<TileBase>()).ToList();

        structure = new NormalStructure();
        ((NormalStructure)structure).Init(neighborBases);
        ((NormalStructure)structure).SetColleagues(colleagueBases);

        landformText = "건물";
        resourceText = "자원 : ???";
        
        for (int i = 0; i < resourceIcons.Length; i++)
        {
            SpriteRenderer item = resourceIcons[i];
            item.sprite = null;
            item.gameObject.SetActive(false);
        }
    }

    public void SetNeighborStructure()
    {
        isStructureNeighbor = true;
    }

    int GetTileDataIndex()
    {
        switch (tileType)
        {
            case ETileType.None:
                return 1001;
            case ETileType.Desert:
                return 1002;
            case ETileType.Tundra:
                return 1003;
            case ETileType.Jungle:
                return 1004;
            case ETileType.Neo:
                return 1005;
        }

        return 0;
    }
    
    public void UpdateZombieInfo(ZombieBase zombie)
    {
        if (zombie == null)
        {
            curZombies = null;
        }
        else
        {
            curZombies = zombie;
        }
    }
}