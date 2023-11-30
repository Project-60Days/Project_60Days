using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectController : MonoBehaviour
{
    [SerializeField] Button buttonA;
    [SerializeField] Button buttonB;

    Dictionary<string, SelectBase> dic_Select = new Dictionary<string, SelectBase>();




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

    public void SetSelect(string _Key)
    {
        if (dic_Select.TryGetValue(_Key, out var selectBase))
        {
            OpenSelectPanel();

            selectBase.SetOptionA(buttonA);
            selectBase.SetOptionB(buttonB);

            buttonA.onClick.RemoveAllListeners();
            buttonB.onClick.RemoveAllListeners();

            buttonA.onClick.AddListener(selectBase.SelectA);
            buttonB.onClick.AddListener(selectBase.SelectB);
        }
    }

    void OpenSelectPanel()
    {
        UIManager.instance.AddCurrUIName("UI_SELECT");
        gameObject.SetActive(true);
    }
}
