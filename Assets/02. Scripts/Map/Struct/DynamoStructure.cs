using System.Collections;
using System.Collections.Generic;
using Hexamap;

public class DynamoStructure : StructBase
{
    protected override string GetCode() => "STRUCT_DYNAMO";

    public override void Init(List<Tile> _colleagueList)
    {
        base.Init(_colleagueList);

        isUse = true;
    }

    public override void YesFunc()
    {
        for (var index = 0; index < colleagueBases.Count; index++)
        {
            var tile = colleagueBases[index];
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