using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Select_JustoUda : SelectBase
{
    [SerializeField] Sprite imageA;
    [SerializeField] Sprite imageB;
    [SerializeField] string textA;
    [SerializeField] string textB;

    public Select_JustoUda()
    {
        key = "JustoUda";
    }

    public override void SelectA()
    {
        if (App.Manager.UI.isUIStatus(UIState.Select))
            App.Manager.UI.PopCurrUI();
        else return;

        Debug.Log("A ��ư ����");
        gameObject.SetActive(false);
    }

    public override void SelectB()
    {
        if (App.Manager.UI.isUIStatus(UIState.Select))
            App.Manager.UI.PopCurrUI();
        else return;

        Debug.Log("B ��ư ����");
        gameObject.SetActive(false);
    }

    public override void SetOptionA(Button _button)
    {
        _button.GetComponent<Image>().sprite = imageA;
        _button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = textA;
    }

    public override void SetOptionB(Button _button)
    {
        _button.GetComponent<Image>().sprite = imageB;
        _button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = textB;
    }
}
