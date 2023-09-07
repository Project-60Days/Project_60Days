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
                var itemName = itemSO.items.ToList().Find(x => x.itemCode == item.itemCode).data.Korean;
                Debug.LogFormat("ÀÚ¿ø ÀÌ¸§ : {0}, ÀÚ¿ø °¹¼ö : {1}", itemName, item.itemCount);
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

                    PlaySFX(itemName);
                    Debug.LogFormat("{0} ÀÚ¿ø, {1}°³ È¹µæ!", itemName, list[i].itemCount);
                }
                else
                {
                    owendResources.Add(list[i]);
                    var itemName = itemSO.items.ToList().Find(x => x.itemCode == owendResources[owendResources.Count - 1].itemCode).data.Korean;
                    PlaySFX(itemName);
                    Debug.LogFormat(itemName + " ÀÚ¿ø {0}°³ Ãß°¡", owendResources[owendResources.Count - 1].itemCount);
                }
            }
        }

        for (int i = 0; i < list.Count; i++)
        {
            ItemBase item = itemSO.items.ToList().Find(x => x.itemCode == list[i].itemCode);

            for (int j = 0; j < list[i].itemCount; j++)
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
                    case "°­Ã¶":
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
