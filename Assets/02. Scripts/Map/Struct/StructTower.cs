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
        App.Manager.UI.GetPanel<PagePanel>().SetNextPage(PageType.Result, "STR_RESULT_STRUCT_YES", data.Korean);
    }

    public override void NoFunc() { }
}
