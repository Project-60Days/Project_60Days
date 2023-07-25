using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Hexamap;

public class Resource
{
    public Resource(EResourceType type, int count, Sprite sprite)
    {
        this.type = type;
        this.count = count;
        this.sprite = sprite;
    }

    public EResourceType type;
    public int count;
    public Sprite sprite;

    public void IncreaseDecrease(int count)
    {
        this.count += count;

        if (this.count < 0)
            this.count = 0;
    }
}

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
    List<EResourceType> eResourceTypes;
    List<Resource> appearanceResources;
    Tile myTile;

    string buildingID;
    string specialID;
    int resourceID;
    bool isCanMove;
    bool inPlayerSight = false;

    public List<Resource> GetResources(int count)
    {
        List<Resource> list = new List<Resource>();

        if (appearanceResources == null)
            Debug.Log("비어있음");

        for (int i = 0; i < appearanceResources.Count; i++)
        {
            Resource item = appearanceResources[i];
            if (item.count - count >= 0)
            {
                list.Add(new Resource(item.type, count, item.sprite));
                item.IncreaseDecrease(-count);
            }
            else
            {
                list.Add(new Resource(item.type, item.count, item.sprite));
                item.IncreaseDecrease(-count);
            }
        }
        ResourceUpdate(true);
        return list;
    }

    void Start()
    {
        MapController.PlayerBehavior += CheckPlayerTIle;
        appearanceResources = new List<Resource>();
        eResourceTypes = new List<EResourceType>() { EResourceType.PLASTIC, EResourceType.STEEL, EResourceType.PLAZMA };
        myTile = gameObject.transform.GetComponent<TileController>().Model;
        RandomResourceUpdate();
    }

    void OnDestroy()
    {
        MapController.PlayerBehavior -= CheckPlayerTIle;
    }

    void RandomResourceUpdate()
    {
        EResourceType type = 0;
        var random = Random.Range(1, 3);


        for (int i = 0; i < random; i++)
        {
            var randomPick = Random.Range(0, eResourceTypes.Count);
            var randomSprite = itemSO.items[randomPick].itemImage;
            type = eResourceTypes[randomPick];
            eResourceTypes.RemoveAt(randomPick);

            var randomCount = Random.Range(1, 16);
            appearanceResources.Add(new Resource(type, randomCount, randomSprite));
        }
    }

    void ResourceUpdate(bool isNearth)
    {
        RotationCheck(transform.rotation.eulerAngles);
        if (isNearth)
        {
            if (appearanceResources.Count == 2)
            {
                for (int i = 0; i < appearanceResources.Count; i++)
                {
                    SpriteRenderer item = resourceIcons[i + 1];
                    item.sprite = appearanceResources[i].sprite;
                    item.gameObject.SetActive(true);
                }

                resourceText.text = appearanceResources[0].type.ToString() + " " + appearanceResources[0].count + "\n"
                    + appearanceResources[1].type.ToString() + " " + appearanceResources[1].count;
            }
            else
            {
                resourceIcons[0].sprite = appearanceResources[0].sprite;
                resourceIcons[0].gameObject.SetActive(true);
                resourceText.text = appearanceResources[0].type.ToString() + " " + appearanceResources[0].count;
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
