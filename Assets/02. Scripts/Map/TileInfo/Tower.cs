public class Tower : StructureBase
{
    protected override string GetCode() => "STRUCT_TOWER";

    public override void YesFunc()
    {
        // 맵 씬 강제 이동 + 조사 애니메이션
        isUse = true;

        App.Manager.UI.GetPanel<PagePanel>().SetResultPage("Signal_Yes", false);
        App.Manager.UI.GetPanel<PagePanel>().CreateSelectDialogueRunner("sequence");
        App.Manager.UI.GetPanel<PagePanel>().isClickYesBtnInTower = true;
    }

    public override void NoFunc()
    {
        // 게임 오버
        App.Manager.Map.ResearchCancel(this);
    }
}
