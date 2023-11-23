using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hexamap;
using System.Linq;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] int collectiveAbility;
    [SerializeField] ItemSO itemSO;

    private List<Resource> owendResources;
    private List<Resource> lastResources;
    bool isGetResource;
    public bool IsGetResource
    {
        get { return isGetResource; }
        set { isGetResource = value; }
    }
    void Start()
    {
        owendResources = new List<Resource>();
        lastResources = new List<Resource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && owendResources != null)
        {
            foreach (var item in owendResources)
            {
                var itemName = itemSO.items.ToList().Find(x => x.data.English == item.ItemCode).data.Korean;
                Debug.LogFormat("이름 : {0}, 개수 : {1}", itemName, item.ItemCount);
            }
        }
    }

    public void GetResource(TileController tile)
    {
        lastResources = tile.GetComponent<TileBase>().GetResources(collectiveAbility);

        for (int i = 0; i < lastResources.Count; i++)
        {
            if (owendResources.Exists(x => x.ItemCode == lastResources[i].ItemCode))
            {
                var resource = owendResources.Find(x => x.ItemCode == lastResources[i].ItemCode);

                if (resource.ItemCount <= 0)
                    return;
                else
                    resource.ItemCount += lastResources[i].ItemCount;

                var item = itemSO.items.ToList().Find(x => x.data.English == resource.ItemCode);
                PlaySFX(item.sfxName);

                Debug.LogFormat("{0} 자원 {1}개 획득", item.data.Korean, lastResources[i].ItemCount);
 
                UIManager.instance.GetPageController().SetResultPage("ITEM_CARBON_None4");
                isGetResource = true;
            }
            else
            {
                owendResources.Add(lastResources[i]);
                var item = itemSO.items.ToList().Find(x => x.data.English == lastResources[i].ItemCode);
                PlaySFX(item.sfxName);
                Debug.LogFormat("새로운 자원 {0} {1}개 획득했다.", item.data.Korean, lastResources[i].ItemCount);
                UIManager.instance.GetPageController().SetResultPage("ITEM_CARBON_None4");
                isGetResource = true;
            }

        }

        for (int i = 0; i < lastResources.Count; i++)
        {
            ItemBase item = itemSO.items.ToList().Find(x => x.English == lastResources[i].ItemCode);

            for (int j = 0; j < lastResources[i].ItemCount; j++)
            {
                Debug.Log(item);
                Debug.Log(item.English);
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
        return tileController.GetComponent<TileBase>().CheckResources();
    }

    public List<Resource> GetLastResources()
    {
        return lastResources;
    }

}
