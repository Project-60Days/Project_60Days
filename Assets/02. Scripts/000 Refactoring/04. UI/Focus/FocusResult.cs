public class FocusResult : FocusBase
{
    public override bool CheckCondition()
        => App.Manager.UI.GetPanel<InventoryPanel>().CheckItemExist("ITEM_BATTERY");
}
