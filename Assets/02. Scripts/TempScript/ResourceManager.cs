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
                Debug.LogFormat("ÀÌ¸§ : {0}, °³¼ö : {1}", itemName, item.ItemCount);
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

                    var item = itemSO.items.ToList().Find(x => x.itemCode == owendResources[owendResources.Count - 1].ItemCode);

                    PlaySFX(item.sfxName);
                    Debug.LogFormat("»õ·Î¿î ¾ÆÀÌÅÛ {0} {1}°³ È¹µæÇß´Ù.", item.data.Korean, list[i].ItemCount);
                    isGetResource = true;
                }
                else
                {
                    owendResources.Add(list[i]);
                    var item = itemSO.items.ToList().Find(x => x.itemCode == owendResources[owendResources.Count - 1].ItemCode);
                    PlaySFX(item.sfxName);
                    Debug.LogFormat(item.data.Korean + " {0}°³ È¹µæÇß´Ù.", owendResources[owendResources.Count - 1].ItemCount);
                    isGetResource = true;
                }
            }
        }

        for (int i = 0; i < list.Count; i++)
        {
            ItemBase item = itemSO.items.ToList().Find(x => x.itemCode == list[i].ItemCode);

            for (int j = 0; j < list[i].ItemCount; j++)
            {
                //TODO :: SFX Àç»ý, ¾ÆÀÌÅÛ È¹µæ ½ºÅ©¸³Æ® ÃßÃ·
                UIManager.instance.GetInventoryController().AddItem(item);
            }
        }
    }

    public void PlaySFX(string str)
    {
        App.instance.GetSoundManager().PlaySFX(str);
    }

    public bool CheckResource(TileController tileController)
    {
        return tileController.GetComponent<TileInfo>().CheckResources();
    }
}
