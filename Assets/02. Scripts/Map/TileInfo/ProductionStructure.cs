using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ProductionStructure : StructureBase
{
    protected override string GetCode() => "STRUCT_PRODUCTION";

    public override void Init(List<TileBase> _neighborTiles, GameObject _structureModel, ItemSO _itemSO)
    {
        base.Init(_neighborTiles, _structureModel, _itemSO);

        App.Data.Game.itemData.TryGetValue(data.SpecialItem, out ItemData itemData);
        specialItem = itemData;
    }

    public override void YesFunc()
    {
        for (var index = 0; index < colleagues.Count; index++)
        {
            var tile = colleagues[index];
            tile.Structure.AllowAccess();
        }

        App.Manager.Map.NormalStructureResearch(this);
        isUse = true;
        isAccessible = true;
        App.Manager.UI.GetPanel<PagePanel>().CreateSelectDialogueRunner("sequence");
        App.Manager.UI.GetPanel<PagePanel>().isClickYesBtnInProductionStructure = true;
    }

    public override void NoFunc()
    {
        // 접근 불가 장애물 타일로 변경
        isUse = true;
    }
}