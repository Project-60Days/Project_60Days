using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hexamap;
using System.Linq;
using UnityEngine.Experimental.Rendering;
using Random = UnityEngine.Random;
using Unity.VisualScripting;

public class Resource
{
    private string itemCode;
    public string ItemCode { get => itemCode; set => itemCode = value; }
    private int itemCount;
    public int ItemCount
    {
        get => itemCount;
        set
        {
            if (itemCount + value < 0)
                itemCount = 0;
            else
                itemCount += value;
        }
    }

    private ItemBase itemBase;
    public ItemBase ItemBase { get => itemBase; set => itemBase = value; }


    public Resource(string _itemCode, int _itemCount, ItemBase _itemBase)
    {
        this.ItemCode = _itemCode;
        this.ItemCount = _itemCount;
        this.ItemBase = _itemBase;
    }

}

public class TileInfo : MonoBehaviour
{

    #region PrivateVariables    

    [Space(5f)]
    [SerializeField] SpriteRenderer[] resourceIcons;
    [SerializeField] protected ItemSO itemSO;


    [Space(5f)]
    [SerializeField] Sprite landformSprite;
    [SerializeField] string landformName;
    public string landformEnglishName;

    protected List<EResourceType> gachaList = new List<EResourceType>();
    List<Resource> appearanceResources = new List<Resource>();

    protected Tile tileController;
    bool inPlayerSight;

    protected Dictionary<EResourceType, int> gachaRate = new Dictionary<EResourceType, int>();
    protected EResourceType choice;



    string resourceText = "";

    private Structure structure;

    public Structure Structure
    {
        get => structure;
    }

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

    public virtual void Init() { }

    protected void SpawnRandomResource()
    {
        var random = Random.Range(1, 3);

        while (gachaList.Count != random)
        {
            var take = WeightedRandomizer.From(gachaRate).TakeOne();
            if (gachaList.Contains(take) == false)
                gachaList.Add(take);
        }

        for (int i = 0; i < gachaList.Count; i++)
        {
            var item = gachaList[i];

            var itemBase = itemSO.items.ToList()
            .Find(x => x.data.English == item.ToString());

            var randomCount = Random.Range(1, 16);
            var resource = new Resource(item.ToString(), randomCount, itemBase);
            appearanceResources.Add(resource);
        }

        gachaRate.Clear();
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
                    resourceText += appearanceResources[i].ItemBase.data.Korean + " " + appearanceResources[i].ItemCount + "개\n";
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
                resourceIcons[resourceIcons.Length - 1].transform.parent.transform.localEulerAngles = new Vector3(90, -rotationValue.y, 0);

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
        if (App.instance.GetMapManager().mapController.GetTilesInRange(tile, 3).Contains(tileController) || tileController == tile)
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
        App.instance.GetMapManager().mapUIController.UpdateText(ETileInfoTMP.Resource, resourceText);
        App.instance.GetMapManager().mapUIController.UpdateText(ETileInfoTMP.Landform, landformName);
        App.instance.GetMapManager().mapUIController.UpdateImage(landformSprite);
    }

    public void SpawnSignal()
    {
        structure = new Signal();
        structure.Init();

        resourceText = "자원 없음";
        for (int i = 0; i < resourceIcons.Length; i++)
        {
            SpriteRenderer item = resourceIcons[i];
            item.sprite = null;
            item.gameObject.SetActive(false);
        }
    }

    public bool ExistanceStructure()
    {
        return structure != null ? true : false;
    }
}