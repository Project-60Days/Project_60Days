public class StructTower : StructBase
{
    protected override string GetCode() => "STRUCT_TOWER";

    public override void Around()
    {
        if (App.Manager.UI.GetPanel<InventoryPanel>().CheckNetCardUsage()) 
            base.Around();
    }
    public override void YesFunc()
    {
        isUse = true;

        App.Manager.UI.GetPanel<PagePanel>().SetResultPage("Signal_Yes", false);
        App.Manager.UI.GetPanel<PagePanel>().CreateSelectDialogueRunner("sequence");
        App.Manager.UI.GetPanel<PagePanel>().isClickYesBtnInTower = true;
    }

    public override void NoFunc()
    {
        // 게임 오버
        return;
    }
}
