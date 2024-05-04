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

        Debug.Log("A ��ư ����");
    }

    public override void SelectB()
    {
        base.SelectB();

        Debug.Log("B ��ư ����");
    }
}
