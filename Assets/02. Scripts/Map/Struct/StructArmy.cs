public class StructArmy : StructBase
{
    protected override string GetCode() => "STRUCT_ARMY";

    public override void YesFunc()
    {
        base.YesFunc();

        SetCanAccess();
    }
}