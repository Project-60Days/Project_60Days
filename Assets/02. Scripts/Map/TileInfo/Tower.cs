using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : StructureBase
{
    public override void Init(List<TileBase> _neighborTiles, GameObject _structureModel, ItemSO _itemSO)
    {
        structureName = "송신 탑";
        isUse = false;
        isAccessible = false;
        resource = new Resource("Wire", 10);
        neighborTiles = _neighborTiles;
        structureModel = _structureModel;
    }
    
    public override void NoFunc()
    {
        // 게임 오버
        App.Manager.Map.ResearchCancel(this);
    }

    public override void YesFunc()
    {
        // 맵 씬 강제 이동 + 조사 애니메이션
        isUse = true;

        App.Manager.UI.GetPageController().SetResultPage("Signal_Yes", false);
        App.Manager.UI.GetPageController().CreateSelectDialogueRunner("sequence");
        App.Manager.UI.GetPageController().isClickYesBtnInTower = true;
    }
}
