using UnityEngine;

public class Select_JustoUda : SelectBase
{
    public override string Key() => App.Data.Game.GetString("STR_SELECT_JUSTOUDA_KEY");

    protected override string GetTextA() => App.Data.Game.GetString("STR_SELECT_JUSTOUDA_A");
    protected override string GetTextB() => App.Data.Game.GetString("STR_SELECT_JUSTOUDA_B");

    protected override void SelectA()
    {
        base.SelectA();

        Debug.Log("Press Button A");
    }

    protected override void SelectB()
    {
        base.SelectB();

        Debug.Log("Press Button B");
    }
}
