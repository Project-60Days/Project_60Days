using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Hexamap;

public class Resource
{
    public Resource(EResourceType type, int count)
    {
        this.type = type;
        this.count = count;
    }

    public EResourceType type;
    public int count;

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
                list.Add(new Resource(item.type, count));
                item.IncreaseDecrease(-count);
            }
            else
            {
                list.Add(new Resource(item.type, item.count));
                item.IncreaseDecrease(-count);
            }
        }
        TextUpdate(true);
        return list;
    }

    void Start()
    {
        MapController.PlayerBehavior += CheckPlayerTIle;
        appearanceResources = new List<Resource>();
        eResourceTypes = new List<EResourceType>() { EResourceType.배터리, EResourceType.강철, EResourceType.나무 };
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

            type = eResourceTypes[randomPick];
            eResourceTypes.RemoveAt(randomPick);

            var randomCount = Random.Range(1, 16);
            appearanceResources.Add(new Resource(type, randomCount));
        }
    }

    void TextUpdate(bool isNearth)
    {
        if (isNearth)
        {
            if (appearanceResources.Count == 2)
                resourceText.text = appearanceResources[0].type.ToString() + " " + appearanceResources[0].count + "\n"
                    + appearanceResources[1].type.ToString() + " " + appearanceResources[1].count;
            else
                resourceText.text = appearanceResources[0].type.ToString() + " " + appearanceResources[0].count;
        }
        else
        {
            resourceText.text = "자원 : ???";
        }

    }

    void CheckPlayerTIle(Tile tile)
    {
        if (MapController.instance.GetTilesInRange(tile, 3).Contains(myTile) || myTile == tile)
        {
            TextUpdate(true);
            inPlayerSight = true;
        }
        else
        {
            TextUpdate(false);
            inPlayerSight = false;
        }
    }

    public TMP_Text GetZombieText()
    {
        return zombieText;
    }
}
