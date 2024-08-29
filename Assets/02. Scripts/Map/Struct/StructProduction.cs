public class StructProduction : StructBase
{
    protected override string GetCode() => "STRUCT_PRODUCTION";

    public override void YesFunc()
    {
        base.YesFunc();

        SetCanAccess();
    }
}