using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalStructure : StructureBase
{
    public override void Init(List<TileBase> _neighborTiles)
    {
        structureName = "일반 건물";
        isUse = false;
        isAccessible = false;
        resource = new Resource("Wire", 10);
        neighborTiles = _neighborTiles;
    }
    
    public void SetColleagues(List<TileBase> _colleagues)
    {
        colleagues = _colleagues;
    }

    public override void YesFunc()
    {
        foreach (var tile in colleagues)
        {
            ((NormalStructure)tile.Structure).AllowAccess();
        }
        
        App.instance.GetMapManager().ResearchStart(this);
        
        //이후 자원 수집 시 isUse도 true로 변경
    }

    public override void NoFunc()
    {
        // 접근 불가 장애물 타일로 변경
        throw new System.NotImplementedException();
    }
    
    public void AllowAccess()
    {
        isAccessible = true;
    }
}
