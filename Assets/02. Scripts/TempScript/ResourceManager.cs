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
    bool isGetResource;
    public bool IsGetResource
    {
        get { return isGetResource; }
        set { isGetResource = value; }
    }
    void Start()
    {
        owendResources = new List<Resource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && owendResources != null)
        {
            foreach (var item in owendResources)
            {
                var itemName = itemSO.items.ToList().Find(x => x.itemCode == item.ItemCode).data.Korean;
                Debug.LogFormat("�ڿ� �̸� : {0}, �ڿ� ���� : {1}", itemName, item.ItemCount);
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
                if (owendResources.Exists(x => x.ItemCode == list[i].ItemCode))
                {
                    var resource = owendResources.Find(x => x.ItemCode == list[i].ItemCode);

                    if (resource.ItemCount <= 0)
                        return;
                    else
                        resource.ItemCount += list[i].ItemCount;

                    var itemName = itemSO.items.ToList().Find(x => x.itemCode == resource.ItemCode).data.Korean;

                    PlaySFX(itemName);
                    Debug.LogFormat("{0} �ڿ�, {1}�� ȹ��!", itemName, list[i].ItemCount);
                    isGetResource = true;
                }
                else
                {
                    owendResources.Add(list[i]);
                    var itemName = itemSO.items.ToList().Find(x => x.itemCode == owendResources[owendResources.Count - 1].ItemCode).data.Korean;
                    PlaySFX(itemName);
                    Debug.LogFormat(itemName + " �ڿ� {0}�� �߰�", owendResources[owendResources.Count - 1].ItemCount);
                    isGetResource = true;
                }
            }
        }

        for (int i = 0; i < list.Count; i++)
        {
            ItemBase item = itemSO.items.ToList().Find(x => x.itemCode == list[i].ItemCode);

            for (int j = 0; j < list[i].ItemCount; j++)
            {
                UIManager.instance.GetInventoryController().AddItem(item);
            }
        }
    }

    public void PlaySFX(string str)
    {
        App.instance.GetSoundManager().PlaySFX("SFX_Metal_Acquisition");
        /*        switch (str)
                {
                    case "��ö":
                        App.instance.GetSoundManager().PlaySFX("SFX_Metal_Acquisition");
                        break;
                    default:
                        App.instance.GetSoundManager().PlaySFX("SFX_Plasma_Acquisition");
                        break;
                }*/
    }

    public bool CheckResource(TileController tileController)
    {
        return tileController.GetComponent<TileInfo>().CheckResources();
    }
}
