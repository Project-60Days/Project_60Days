public class InteractNote : InteractBase
{
    protected override void OnClickEvent()
    {
        App.Manager.UI.GetPanel<NotePanel>().OpenPanel();
    }
}
