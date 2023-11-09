using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hexamap;
using System.Linq;
using Random = UnityEngine.Random;

public class Resource
{
    private string itemCode;
    private int itemCount;
    public string ItemCode { get => itemCode; set => itemCode = value; }
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

    public Resource(string itemCode, int itemCount)
    {
        this.ItemCode = itemCode;
        this.ItemCount = itemCount;
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

    protected List<EResourceType> gachaList =new List<EResourceType>();
    List<Resource> appearanceResources = new List<Resource>();

    protected Tile tileController;
    bool inPlayerSight;

    protected Dictionary<EResourceType, int> gachaRate = new Dictionary<EResourceType, int>();
    protected EResourceType choice;

    string structureName = "구조물 없음";
    string resourceText = "";

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
            if(gachaList.Contains(take) == false)
                gachaList.Add(take);
        }
        
        for (int i = 0; i < gachaList.Count; i++)
        {
            var item = gachaList[i];

            var randomCount = Random.Range(1, 16);
            var resource = new Resource(item.ToString(), randomCount);
            appearanceResources.Add(resource);
        }
        
        gachaRate.Clear();
    }

    void ResourceUpdate(bool _isInPlayerSight)
    {
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
                    if(isItem == true)
                        itemIcon = resourceIcons[i + 1];
                    else
                    {
                        itemIcon = resourceIcons[i];
                    }
                    
                    var item = itemSO.items.ToList()
                        .Find(x => x.data.English == appearanceResources[i].ItemCode);

                    itemIcon.sprite = item.itemImage;
                    itemIcon.gameObject.SetActive(true);
                    resourceText += item.data.Korean + " " + appearanceResources[i].ItemCount + "개\n";
                }
            }
            else
            {
                resourceText = "자원 : 없음";
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
            Debug.Log("������ �� �ִ� �ڿ� ����.");
            return null;
        }

        for (int i = 0; i < appearanceResources.Count; i++)
        {
            Resource item = appearanceResources[i];
            if (item.ItemCount - count >= 0)
            {
                list.Add(new Resource(item.ItemCode, count));
                item.ItemCount -= count;
            }
            else
            {
                list.Add(new Resource(item.ItemCode, item.ItemCount));
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

    public void SetStructureName(string name)
    {
        structureName = name;
    }

    public string GetStructureName()
    {
        return structureName;
    }

    public void ChangeText()
    {
        App.instance.GetMapManager().mapUIController.UpdateText(ETileInfoTMP.Resource, resourceText);
    }
    
}