using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hexamap;

public class NoneTile : TileInfo
{
    public override void Init()
    {
        tileController = gameObject.transform.GetComponent<TileController>().Model;
        App.instance.GetDataManager().tileData.TryGetValue(1001, out TileData tileData);
        
        gachaRate.Add(EResourceType.Steel, tileData.RemainPossibility_Steel);
        gachaRate.Add(EResourceType.Carbon, tileData.RemainPossibility_Carbon);
        gachaRate.Add(EResourceType.Plasma, tileData.RemainPossibility_Plasma);
        gachaRate.Add(EResourceType.Powder, tileData.RemainPossibility_Powder);
        gachaRate.Add(EResourceType.Gas, tileData.RemainPossibility_Gas);
        gachaRate.Add(EResourceType.Rubber, tileData.RemainPossibility_Rubber);
        
        SpawnRandomResource();
        RotationCheck(transform.rotation.eulerAngles);
    }
}
