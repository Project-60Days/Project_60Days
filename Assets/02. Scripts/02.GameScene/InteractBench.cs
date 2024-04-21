public class InteractBench : InteractObj
{
    protected override void OnClickEvent()
    {
        App.Manager.UI.GetPanel<CraftPanel>().OpenPanel();
    }
}
