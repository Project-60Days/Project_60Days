public class Interact_Note : InteractBase
{
    protected override void OnClickEvent()
    {
        App.Manager.UI.GetPanel<NotePanel>().OpenPanel();
    }
}
