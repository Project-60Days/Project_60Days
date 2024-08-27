using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable]
public struct SelectButton
{
    public Button btn;
    public Image img;
    public TextMeshProUGUI text;
}

public class SelectPanel : UIBase
{
    [SerializeField] SelectButton buttonA;
    [SerializeField] SelectButton buttonB;

    private Dictionary<string, SelectBase> dic_Select = new();

    #region Override
    public override void Init()
    {
        dic_Select.Clear();

        SelectBase[] selectBases = GetComponentsInChildren<SelectBase>();
        foreach (SelectBase selectBase in selectBases)
        {
            dic_Select.Add(selectBase.Key(), selectBase);
        }

        gameObject.SetActive(false);
    }

    public override UIState GetUIState() => UIState.Select;

    public override bool IsAddUIStack() => true;
    #endregion

    public void SetSelect(string _key)
    {
        if (dic_Select.TryGetValue(_key, out var selectBase))
        {
            selectBase.SetOptionA(buttonA);
            selectBase.SetOptionB(buttonB);

            OpenPanel();
        }
    }
}
