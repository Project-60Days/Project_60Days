public class ArmyStructure : StructureBase
{
    protected override string GetCode() => "STRUCT_ARMY";

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