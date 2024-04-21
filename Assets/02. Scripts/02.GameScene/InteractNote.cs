public class InteractNote : InteractObj
{
    protected override void OnClickEvent()
    {
        App.Manager.UI.GetPanel<NotePanel>().OpenPanel();
    }
}
