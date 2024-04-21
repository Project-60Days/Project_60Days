public class InteractMap : InteractObj
{
    protected override void OnClickEvent()
    {
        App.Manager.UI.GetNextDayController().GoToMap();
    }
}