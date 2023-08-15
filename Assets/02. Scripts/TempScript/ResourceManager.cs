using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hexamap;
using System.Linq;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] int collectiveAbility;
    [SerializeField] ItemSO itemSO;

    List<Resource> owendResources;
    InventoryPage inventory;

    void Start()
    {
        StartCoroutine(GetInventoryPage());
        owendResources = new List<Resource>();
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
                var itemName = itemSO.items.ToList().Find(x => x.itemCode == item.itemCode).data.Korean;
                Debug.LogFormat("자원 이름 : {0}, 자원 갯수 : {1}", itemName, item.itemCount);
            }
        }
    }

    public void GetResource(TileController tile)
    {
        var list = tile.GetComponent<TileInfo>().GetResources(collectiveAbility);

        if (owendResources == null)
            owendResources = list;
        else
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (owendResources.Exists(x => x.itemCode == list[i].itemCode))
                {
                    var resource = owendResources.Find(x => x.itemCode == list[i].itemCode);

                    if (resource.itemCount <= 0)
                        return;
                    else
                        resource.itemCount += list[i].itemCount;

                    var itemName = itemSO.items.ToList().Find(x => x.itemCode == resource.itemCode).data.Korean;

                    Debug.LogFormat("{0} 자원, {1}개 획득!", itemName, list[i].itemCount);
                }
                else
                {
                    owendResources.Add(list[i]);
                    var itemName = itemSO.items.ToList().Find(x => x.itemCode == owendResources[owendResources.Count - 1].itemCode).data.Korean;
                    Debug.LogFormat(itemName + " 자원 {0}개 추가", owendResources[owendResources.Count - 1].itemCount);
                }
            }
        }

        for (int i = 0; i < list.Count; i++)
        {
            ItemBase item = itemSO.items.ToList().Find(x => x.itemCode == list[i].itemCode);

            for (int j = 0; j < list[i].itemCount; j++)
            {
                inventory.AddItem(item);
            }
        }
    }
}
