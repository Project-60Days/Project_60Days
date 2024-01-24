using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionStructure : StructureBase
{
    public override void Init(List<TileBase> _neighborTiles,GameObject _structureModel)
    {
        structureName = "생산 건물";
        isUse = false;
        isAccessible = false;
        
        resource = new Resource("Wire", 10);
        neighborTiles = _neighborTiles;
        structureModel = _structureModel;
        
        App.instance.GetDataManager().itemData.TryGetValue("ITEM_NETWORKCHIP", out ItemData itemData);
        specialItem = itemData;
    }

    public void SetColleagues(List<TileBase> _colleagues)
    {
        colleagues = _colleagues;
    }

    public override void YesFunc()
    {
        for (var index = 0; index < colleagues.Count; index++)
        {
            var tile = colleagues[index];
            ((ProductionStructure)tile.Structure).AllowAccess();
        }

        App.instance.GetMapManager().NormalStructureResearch(this);
        isUse = true;
        isAccessible = true;
        UIManager.instance.GetPageController().CreateSelectDialogueRunner("sequence");
    }

    public override void NoFunc()
    {
        // 접근 불가 장애물 타일로 변경
        isUse = true;
    }
    
    public void AllowAccess()
    {
        isUse = true;
        isAccessible = true;
    }
}
