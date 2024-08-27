public class InteractBench : InteractBase
{
    protected override void OnClickEvent()
    {
        App.Manager.UI.GetPanel<CraftPanel>().OpenPanel();
    }
}
