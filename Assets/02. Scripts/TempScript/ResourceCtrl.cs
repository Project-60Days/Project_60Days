using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hexamap;
using System.Linq;

public class ResourceCtrl : MonoBehaviour
{
    [SerializeField] int collectiveAbility;

    private ItemSO itemSO;

    private List<Resource> owendResources;
    private List<Resource> lastResources;

    private bool isGetResource;

    public bool IsGetResource
    {
        get { return isGetResource; }
        set { isGetResource = value; }
    }

    void Start()
    {
        owendResources = new List<Resource>();
        lastResources = new List<Resource>();

        itemSO = App.Manager.Game.itemSO;
    }

    public void GetResource(TileController tile)
    {
        int collectiveCount = collectiveAbility;

        if (App.Manager.Map.IsJungleTile(tile))
            collectiveCount += 1;

        lastResources = tile.GetComponent<TileBase>().GetResources(collectiveAbility);

        for (int i = 0; i < lastResources.Count; i++)
        {
            if (owendResources.Exists(x => x.Item == lastResources[i].Item))
            {
                var resource = owendResources.Find(x => x.Item == lastResources[i].Item);

                if (resource.Count <= 0)
                    return;
                else
                    resource.Count += lastResources[i].Count;

                var item = itemSO.items.ToList().Find(x => x == resource.Item);

                //Debug.LogFormat("{0} 자원 {1}개 획득", item.data.Korean, lastResources[i].ItemCount);
                isGetResource = true;
            }
            else
            {
                owendResources.Add(lastResources[i]);
                var item = itemSO.items.ToList().Find(x => x == lastResources[i].Item);

                //Debug.LogFormat("새로운 자원 {0} {1}개 획득했다.", item.data.Korean, lastResources[i].ItemCount);
                isGetResource = true;
            }
        }

        for (int i = 0; i < lastResources.Count; i++)
        {
            ItemBase item = itemSO.items.ToList().Find(x => x == lastResources[i].Item);

            for (int j = 0; j < lastResources[i].Count; j++)
            {
                //TODO :: SFX 재생, 아이템 획득 스크립트 추첨
                App.Manager.UI.GetPanel<InventoryPanel>().AddItem(item);
                App.Manager.Sound.PlaySFX(item.sfxName);
            }
        }
    }

    public bool CheckResource(TileController tileController)
    {
        return tileController.GetComponent<TileBase>().CheckResources();
    }

    public List<Resource> GetLastResources()
    {
        if (lastResources != null || lastResources.Count > 0)
        {
            return lastResources;
        }
        else
        {
            return null;
        }
    }
}