public class FocusItem : FocusBase
{
    public override bool CheckCondition()
        => App.Manager.UI.GetPanel<BenchPanel>().Craft.IsCombinedResult;

    public override void OnFinish()
    {
        App.Manager.UI.GetPanel<FocusPanel>().ShowFocus("ResultItem");
    }
}
