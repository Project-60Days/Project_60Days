public class StructArmy : StructBase
{
    protected override string GetCode() => "STRUCT_ARMY";

    public override void YesFunc()
    {
        for (var index = 0; index < colleagueBases.Count; index++)
        {
            var tile = colleagueBases[index];
            tile.structure.AllowAccess();
        }

        App.Manager.Map.NormalStructureResearch(this);

        FadeIn();
        colleagueBases.ForEach(tile => tile.ResourceUpdate(true));

        isUse = true;
        isAccessible = true;

        App.Manager.UI.GetPanel<PagePanel>().CreateSelectDialogueRunner("sequence");
    }

    public override void NoFunc()
    {
        isUse = true;
    }
}