using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Hexamap;
using System.Linq;

public class Resource
{
    public string itemCode;
    public int itemCount;

    public Resource(string itemCode, int itemCount)
    {
        this.itemCode = itemCode;
        this.itemCount = itemCount;
    }

    public void SetCount(int count)
    {
        itemCount = count;
    }

    public void IncreaseDecreaseCount(int count)
    {
        itemCount += count;

        if (itemCount < 0)
            itemCount = 0;
    }
}

public class TileInfo : MonoBehaviour
{
    [Header("컴포넌트")]
    [Space(5f)]
    [SerializeField] SpriteRenderer[] resourceIcons;
    [SerializeField] ItemSO itemSO;

    [Header("지형 정보")]
    [Space(5f)]
    [SerializeField] Sprite landformSprite;

    List<ItemBase> gachaList;
    List<Resource> appearanceResources;
    
    Tile myTile;

    bool inPlayerSight = false;
    bool isTutorialTile = false;

    // 수정 필요

    void Start()
    {
        Player.PlayerSightUpdate += CheckPlayerTIle;
        myTile = gameObject.transform.GetComponent<TileController>().Model;
        appearanceResources = new List<Resource>();
        gachaList = new List<ItemBase>();
        for (int i = 0; i < itemSO.items.Length; i++)
        {
            gachaList.Add(itemSO.items[i]);
        }

        /*        if (CheckTutorial(MapController.instance.GetPlayerLocationTile()))
                {
                    TutorialResourceUpdate();
                }
                else
                {
                    RandomResourceUpdate();
                }*/

        RandomResourceUpdate();
        RotationCheck(transform.rotation.eulerAngles);
    }

    void OnDestroy()
    {
        Player.PlayerSightUpdate -= CheckPlayerTIle;
    }

    void RandomResourceUpdate()
    {
        var random = Random.Range(1, 3);

        for (int i = 0; i < random; i++)
        {
            var randomPick = Random.Range(0, gachaList.Count);
            var item = gachaList[randomPick];

            var randomCount = Random.Range(1, 16);
            var resource = new Resource(item.itemCode, randomCount);

            appearanceResources.Add(resource);
            gachaList.RemoveAt(randomPick);
        }
    }

    void ResourceUpdate(bool isNearth)
    {
        if (isNearth)
        {

            for (int i = 0; i < appearanceResources.Count; i++)
            {
                Resource item = appearanceResources[i];
                if (item.itemCount == 0)
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

            if (appearanceResources.Count == 2)
            {
                for (int i = 0; i < appearanceResources.Count; i++)
                {
                    SpriteRenderer item = resourceIcons[i + 1];
                    var itemImage = itemSO.items.ToList().Find(x => x.itemCode == appearanceResources[i].itemCode).itemImage;
                    item.sprite = itemImage;
                    item.gameObject.SetActive(true);
                }

                var itemName1 = itemSO.items.ToList().Find(x => x.itemCode == appearanceResources[0].itemCode).data.Korean;
                var itemName2 = itemSO.items.ToList().Find(x => x.itemCode == appearanceResources[1].itemCode).data.Korean;
                var text = itemName1 + " " + appearanceResources[0].itemCount + "\n"
                    + itemName2 + " " + appearanceResources[1].itemCount;

                App.instance.GetMapManager().mapUIController.UpdateText(ETileInfoTMP.Resource, text);
            }
            else if (appearanceResources.Count == 1)
            {
                var itemName1 = itemSO.items.ToList().Find(x => x.itemCode == appearanceResources[0].itemCode).data.Korean;

                resourceIcons[0].sprite = itemSO.items.ToList().Find(x => x.itemCode == appearanceResources[0].itemCode).itemImage;
                resourceIcons[0].gameObject.SetActive(true);

                var text = itemName1 + " " + appearanceResources[0].itemCount;
                App.instance.GetMapManager().mapUIController.UpdateText(ETileInfoTMP.Resource, text);
            }
            else
            {
                App.instance.GetMapUiController().UpdateText(ETileInfoTMP.Resource, "자원 : 없음");
                for (int i = 0; i < resourceIcons.Length; i++)
                {
                    SpriteRenderer item = resourceIcons[i];
                    item.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            App.instance.GetMapManager().mapUIController.UpdateText(ETileInfoTMP.Resource, "자원 : ???");

            for (int i = 0; i < resourceIcons.Length; i++)
            {
                SpriteRenderer item = resourceIcons[i];
                item.gameObject.SetActive(false);
            }
        }
    }

    void RotationCheck(Vector3 rotationValue)
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
            Debug.Log("비어있음");

        for (int i = 0; i < appearanceResources.Count; i++)
        {
            Resource item = appearanceResources[i];
            if (item.itemCount - count >= 0)
            {
                list.Add(new Resource(item.itemCode, count));
                item.IncreaseDecreaseCount(-count);
            }
            else
            {
                list.Add(new Resource(item.itemCode, item.itemCount));
                item.IncreaseDecreaseCount(-count);
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
        if (MapController.instance.GetTilesInRange(tile, 3).Contains(myTile) || myTile == tile)
        {
            ResourceUpdate(true);
            inPlayerSight = true;
        }
        else
        {
            ResourceUpdate(false);
            inPlayerSight = false;
        }
    }

    bool CheckTutorial(Tile tile)
    {
        if (MapController.instance.GetTilesInRange(tile, 1).Contains(myTile) || myTile == tile)
        {
            isTutorialTile = true;
            return true;
        }
        else
        {
            isTutorialTile = false;
            return false;
        }
    }
}