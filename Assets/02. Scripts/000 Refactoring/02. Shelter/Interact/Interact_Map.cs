public class Interact_Map : InteractBase
{
    protected override void OnClickEvent()
    {
        App.Manager.Event.PostEvent(EventCode.GoToMap, this);
    }
}