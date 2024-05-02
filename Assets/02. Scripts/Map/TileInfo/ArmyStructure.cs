using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ArmyStructure : StructureBase
{
    public override void Init(List<TileBase> _neighborTiles, GameObject _structureModel, ItemSO _itemSO)
    {
        structureName = "군사 시설";

        resource = new Resource("ITEM_DISTURBE", 2);
        isUse = false;
        isAccessible = false;

        neighborTiles = _neighborTiles;
        structureModel = _structureModel;
    }


    public override void NoFunc()
    {
        isUse = true;
    }

    public override void YesFunc()
    {
        for (var index = 0; index < colleagues.Count; index++)
        {
            var tile = colleagues[index];
            ((ArmyStructure)tile.Structure).AllowAccess();
        }

        App.Manager.Map.NormalStructureResearch(this);

        isUse = true;
        isAccessible = true;

        App.Manager.UI.GetPanel<PagePanel>().CreateSelectDialogueRunner("sequence");
    }
}