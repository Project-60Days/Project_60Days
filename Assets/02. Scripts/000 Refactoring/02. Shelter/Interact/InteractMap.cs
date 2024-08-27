public class InteractMap : InteractBase
{
    protected override void OnClickEvent()
    {
        App.Manager.Event.PostEvent(EventCode.GoToMap, this);
    }
}