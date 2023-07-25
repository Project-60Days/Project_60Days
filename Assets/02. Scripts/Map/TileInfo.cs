using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Hexamap;
using System.Linq;

public class TileInfo : MonoBehaviour
{
    [SerializeField] TMP_Text resourceText;
    [SerializeField] TMP_Text zombieText;
    [SerializeField] ItemSO itemSO;
    [SerializeField] GameObject player;
    [SerializeField] GameObject ui;
    [SerializeField] SpriteRenderer[] resourceIcons;

    ETileType tileType;
    EWeatherType weatherType;
    List<ItemBase> gachaList;
    List<ItemBase> appearanceResources;
    Tile myTile;

    string buildingID;
    string specialID;
    int resourceID;
    bool isCanMove;
    bool inPlayerSight;

    // 수정 필요
    public List<ItemBase> GetResources(int count)
    {
        List<ItemBase> list = new List<ItemBase>();

        if (appearanceResources == null)
            Debug.Log("비어있음");

        for (int i = 0; i < appearanceResources.Count; i++)
        {
            ItemBase item = appearanceResources[i];
            if (item.itemCount - count >= 0)
            {
                var newItem = item;
                newItem.SetCount(2);
                list.Add(newItem);
                item.IncreaseDecreaseCount(-count);
            }
            else
            {
                list.Add(item);
                item.IncreaseDecreaseCount(-count);
            }
        }
        ResourceUpdate(true);
        return list;
    }

    void Start()
    {
        MapController.PlayerBehavior += CheckPlayerTIle;
        myTile = gameObject.transform.GetComponent<TileController>().Model;
        appearanceResources = new List<ItemBase>();
        gachaList = new List<ItemBase>();
        for (int i = 0; i < itemSO.items.Length; i++)
        {
            gachaList.Add(itemSO.items[i]);
        }
        RandomResourceUpdate();
        RotationCheck(transform.rotation.eulerAngles);
    }

    void OnDestroy()
    {
        MapController.PlayerBehavior -= CheckPlayerTIle;
    }

    void RandomResourceUpdate()
    {
        var random = Random.Range(1, 3);
        ItemBase item = new ItemBase();

        for (int i = 0; i < random; i++)
        {
            var randomPick = Random.Range(0, gachaList.Count);
            item = gachaList[randomPick];

            appearanceResources.Add(item);
            gachaList.RemoveAt(randomPick);
        }
    }

    void ResourceUpdate(bool isNearth)
    {
        if (isNearth)
        {

            for (int i = 0; i < appearanceResources.Count; i++)
            {
                ItemBase item = appearanceResources[i];
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
                    item.sprite = appearanceResources[i].itemImage;
                    item.gameObject.SetActive(true);
                }

                resourceText.text = appearanceResources[0].resourceType.ToString() + " " + appearanceResources[0].itemCount + "\n"
                    + appearanceResources[1].resourceType.ToString() + " " + appearanceResources[1].itemCount;
            }
            else if(appearanceResources.Count == 1)
            {
                resourceIcons[0].sprite = appearanceResources[0].itemImage;
                resourceIcons[0].gameObject.SetActive(true);
                resourceText.text = appearanceResources[0].resourceType.ToString() + " " + appearanceResources[0].itemCount;
            }
            else
            {
                resourceText.text = "자원 : 없음";
                for (int i = 0; i < resourceIcons.Length; i++)
                {
                    SpriteRenderer item = resourceIcons[i];
                    item.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            resourceText.text = "자원 : ???";
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
                resourceIcons[resourceIcons.Length-1].transform.parent.transform.localEulerAngles = new Vector3(90,-rotationValue.y,0);

                for (int i = 0; i < resourceIcons.Length; i++)
                {
                    SpriteRenderer item = resourceIcons[i];
                    item.gameObject.transform.Rotate(0, 0, 90);
                }
            }
        }
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

    public TMP_Text GetZombieText()
    {
        return zombieText;
    }
}
