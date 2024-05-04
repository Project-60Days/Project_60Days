using UnityEngine;

public class Select_JustoUda : SelectBase
{
    public Select_JustoUda()
    {
        Key = "JustoUda";
    }

    public override void SelectA()
    {
        base.SelectA();

        Debug.Log("A 버튼 눌림");
    }

    public override void SelectB()
    {
        base.SelectB();

        Debug.Log("B 버튼 눌림");
    }
}
