public class Interact_Bench : InteractBase
{
    protected override void OnClickEvent()
    {
        App.Manager.UI.GetPanel<CraftPanel>().OpenPanel();
    }
}
