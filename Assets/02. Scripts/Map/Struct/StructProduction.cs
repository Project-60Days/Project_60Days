using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Hexamap;

public class StructProduction : StructBase
{
    protected override string GetCode() => "STRUCT_PRODUCTION";

    public override void Init(List<Tile> _colleagueList)
    {
        base.Init(_colleagueList);

        App.Data.Game.itemData.TryGetValue(data.SpecialItem, out ItemData itemData);
        specialItem = itemData;
    }

    public override void YesFunc()
    {
        for (var index = 0; index < colleagueBases.Count; index++)
        {
            var tile = colleagueBases[index];
            tile.structure.AllowAccess();
        }

        FadeIn();
        colleagueBases.ForEach(tile => tile.ResourceUpdate(true));

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