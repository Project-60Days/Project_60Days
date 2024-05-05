using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DynamoStructure : StructureBase
{
    protected override string GetCode() => "STRUCT_DYNAMO";

    public override void Init(List<TileBase> _neighborTiles, GameObject _structureModel, ItemSO _itemSO)
    {
        base.Init(_neighborTiles, _structureModel, _itemSO);

        isUse = true;
    }

    public override void YesFunc()
    {
        for (var index = 0; index < colleagues.Count; index++)
        {
            var tile = colleagues[index];
            tile.structure.AllowAccess();
        }

        App.Manager.Map.NormalStructureResearch(this);

        isUse = true;
        isAccessible = true;

        App.Manager.UI.GetPanel<PagePanel>().CreateSelectDialogueRunner("sequence");
    }

    public override void NoFunc()
    {
        isUse = true;
    }
}