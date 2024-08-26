public class FocusItem : FocusBase
{
    public override bool CheckCondition()
        => App.Manager.UI.GetPanel<CraftPanel>().Craft.IsCombinedResult;
}
