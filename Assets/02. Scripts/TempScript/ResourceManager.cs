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
                var itemName = itemSO.items.ToList().Find(x => x.English == item.ItemCode).data.Korean;
                Debug.LogFormat("이름 : {0}, 개수 : {1}", itemName, item.ItemCount);
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

                    var item = itemSO.items.ToList().Find(x => x.English == owendResources[owendResources.Count - 1].ItemCode);

                    PlaySFX(item.sfxName);
                    Debug.LogFormat("새로운 아이템 {0} {1}개 획득했다.", item.data.Korean, list[i].ItemCount);
                    isGetResource = true;
                }
                else
                {
                    owendResources.Add(list[i]);
                    var item = itemSO.items.ToList().Find(x => x.English == owendResources[owendResources.Count - 1].ItemCode);
                    PlaySFX(item.sfxName);
                    Debug.LogFormat(item.data.Korean + " {0}개 획득했다.", owendResources[owendResources.Count - 1].ItemCount);
                    isGetResource = true;
                }
            }
        }

        for (int i = 0; i < list.Count; i++)
        {
            ItemBase item = itemSO.items.ToList().Find(x => x.English == list[i].ItemCode);

            for (int j = 0; j < list[i].ItemCount; j++)
            {
                //TODO :: SFX 재생, 아이템 획득 스크립트 추첨
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
