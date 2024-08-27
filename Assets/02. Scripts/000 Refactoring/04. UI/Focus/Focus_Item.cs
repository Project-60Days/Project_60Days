public class Focus_Item : FocusBase
{
    public override bool CheckCondition()
        => App.Manager.UI.GetPanel<CraftPanel>().Craft.IsCombinedResult;

    public override void OnFinish()
    {
        App.Manager.UI.GetPanel<FocusPanel>().ShowFocus("ResultItem");
    }
}
