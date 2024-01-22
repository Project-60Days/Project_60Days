using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : StructureBase
{
    public override void Init(List<TileBase> _neighborTiles)
    {
        structureName = "신호기";
        isUse = false;
        isAccessible = false;
        resource = new Resource("Wire", 10);
        neighborTiles = _neighborTiles;
    }
    
    public override void NoFunc()
    {
        // 게임 오버
        App.instance.GetMapManager().ResearchCancel(this);
    }

    public override void YesFunc()
    {
        // 맵 씬 강제 이동 + 조사 애니메이션
        isUse = true;
        isAccessible = true;
        App.instance.GetMapManager().ResearchStart(this);
        UIManager.instance.GetPageController().SetResultPage("Signal_Yes", false);
        UIManager.instance.GetPageController().CreateSelectDialogueRunner("signalSequence");
    }
}
