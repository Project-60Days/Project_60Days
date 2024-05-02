public class InteractMap : InteractBase
{
    protected override void OnClickEvent()
    {
        App.Manager.Game.GoToMap();
    }
}