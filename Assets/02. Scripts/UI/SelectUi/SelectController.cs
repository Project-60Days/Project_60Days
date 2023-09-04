using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectController : ControllerBase
{
    [SerializeField] Button buttonA;
    [SerializeField] Button buttonB;

    Dictionary<string, SelectBase> dic_Select = new Dictionary<string, SelectBase>();





    public override EControllerType GetControllerType()
    {
        return EControllerType.SELECT;
    }





    void Start()
    {
        Init();
    }

    void Init()
    {
        dic_Select.Clear();

        SelectBase[] selectBases = GetComponents<SelectBase>();
        foreach(SelectBase selectBase in selectBases)
        {
            dic_Select.Add(selectBase.key, selectBase);
        }

        gameObject.SetActive(false);
    }

    public void SetSelect(string Key)
    {
        if (dic_Select.TryGetValue(Key, out var selectBase))
        {
            OpenSelectPanel();

            selectBase.GetOptionA(buttonA);
            selectBase.GetOptionB(buttonB);

            buttonA.onClick.RemoveAllListeners();
            buttonB.onClick.RemoveAllListeners();

            buttonA.onClick.AddListener(selectBase.SelectA);
            buttonB.onClick.AddListener(selectBase.SelectB);
        }
    }

    void OpenSelectPanel()
    {
        gameObject.SetActive(true);
    }
}
