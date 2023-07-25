using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hexamap;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] int collectiveAbility;
    Tile currenTile;
    TileInfo currenTileInfo;
    List<ItemBase> owendResources;
    InventoryPage inventory;

    void Start()
    {
        StartCoroutine(GetInventoryPage());
        owendResources = new List<ItemBase>();
    }

    IEnumerator GetInventoryPage()
    {
        yield return new WaitForEndOfFrame();
        inventory = GameObject.FindGameObjectWithTag("UiCanvas").transform.Find("InventoryUi").transform.Find("Inventory").GetComponent<InventoryPage>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && owendResources != null)
        {
            foreach (var item in owendResources)
            {
                Debug.LogFormat("자원 이름 : {0}, 자원 갯수 : {1}", item.resourceType.ToString(), item.itemType);
            }
        }
    }

    public void SetTile(Tile tile)
    {
        currenTile = tile;
        currenTileInfo = ((GameObject)(currenTile.GameEntity)).GetComponent<TileInfo>();
    }

    public void GetResource()
    {
        var list = currenTileInfo.GetResources(collectiveAbility);

        if (owendResources == null)
            owendResources = list;
        else
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (owendResources.Exists(x => x.resourceType == list[i].resourceType))
                {
                    var resource = owendResources.Find(x => x.resourceType == list[i].resourceType);

                    if (resource.itemCount <= 0)
                        return;
                    else
                        resource.itemCount += list[i].itemCount;

                    Debug.LogFormat("{0} 자원, {1}개 획득!", resource.resourceType.ToString(), list[i].resourceType);
                }
                else
                {
                    owendResources.Add(list[i]);
                    Debug.Log(owendResources[owendResources.Count - 1].resourceType.ToString() + " 자원 2개 추가");
                }
            }
        }

        for (int i = 0; i < owendResources.Count; i++)
        {
            ItemBase item = owendResources[i];
            inventory.AddItem(item);
        }
    }
}
