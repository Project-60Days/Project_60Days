using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hexamap;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] int collectiveAbility;
    Tile currenTile;
    TileInfo currenTileInfo;
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
                Debug.LogFormat("자원 이름 : {0}, 자원 갯수 : {1}", item.type.ToString(), item.count);
            }
        }
    }

    public void SetTile(Tile tile)
    {
        currenTile = tile;
        currenTileInfo = ((GameObject)(currenTile.GameEntity)).transform.Find("Canvas").Find("TileInfo").GetComponent<TileInfo>();
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
                if (owendResources.Exists(x => x.type == list[i].type))
                {
                    owendResources.Find(x => x.type == list[i].type).count += list[i].count;
                    Debug.Log(owendResources.Find(x => x.type == list[i].type).type.ToString() + "자원 스택 쌓임");
                }
                else
                {
                    owendResources.Add(list[i]);
                    Debug.Log(owendResources[owendResources.Count - 1].type.ToString() + "자원 추가");
                }
            }
        }
    }
}
