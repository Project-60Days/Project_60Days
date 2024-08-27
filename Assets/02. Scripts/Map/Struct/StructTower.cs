public class StructTower : StructBase
{
    protected override string GetCode() => "STRUCT_TOWER";

    public override void DetectStruct()
    {
        if (App.Manager.UI.GetPanel<InventoryPanel>().CheckItemExist("ITEM_NETWORKCHIP"))
        {
            base.DetectStruct();
            App.Manager.UI.GetPanel<InventoryPanel>().RemoveItemByCode("ITEM_NETWORKCHIP");
        }
    }
    public override void YesFunc()
    {
        App.Manager.UI.GetPanel<PagePanel>().SetResultPage("Signal_Yes", false);
        App.Manager.UI.GetPanel<PagePanel>().CreateSelectDialogueRunner("sequence");
    }

    public override void NoFunc() { }
}
