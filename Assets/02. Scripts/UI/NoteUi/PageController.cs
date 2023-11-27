using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageController : MonoBehaviour
{
    [SerializeField] Button yesBtn;
    [SerializeField] Button noBtn;
    NotePageBase resultPage;
    NotePageBase selectPage;


    void Awake()
    {
        NotePageBase[] pages = GetComponentsInChildren<NotePageBase>(includeInactive: true);
        foreach (var page in pages)
        {
            if (page.GetENotePageType() == ENotePageType.Result)
                resultPage = page;
            else if (page.GetENotePageType() == ENotePageType.Select)
                selectPage = page;
        }
    }

    public void SetResultPage(string _nodeName, bool _isResourceNode)
    {
        resultPage.SetNodeName(_nodeName, _isResourceNode);
    }

    public void SetSelectPage(string _nodeName, StructureBase _structData)
    {
        selectPage.SetNodeName(_nodeName);

        yesBtn.onClick.RemoveAllListeners();
        noBtn.onClick.RemoveAllListeners();

        yesBtn.onClick.AddListener(_structData.YesFunc);
        noBtn.onClick.AddListener(_structData.NoFunc);

        yesBtn.onClick.AddListener(UIManager.instance.GetNoteController().CloseNote);
        noBtn.onClick.AddListener(UIManager.instance.GetNoteController().CloseNote);
    }

    public void SetTutorialSelect()
    {
        selectPage.SetNodeName("tutorialSelect");
        UIManager.instance.GetNoteController().dayCount = -1;
        UIManager.instance.GetNoteController().SetNextDay();
        yesBtn.onClick.RemoveAllListeners();
        noBtn.onClick.RemoveAllListeners();

        yesBtn.onClick.AddListener(TutorialYesFunc);
    }

    public void TutorialYesFunc()
    {
        TutorialManager.instance.GetTutorialController().LightUpBackground();
        UIManager.instance.GetInventoryController().RemoveItemByCode("ITEM_BATTERY");
        UIManager.instance.GetNoteController().CloseNote();
    }

    public string GetNextResourceNodeName()
    {
        if (resultPage.todayResourceNodeNames.Count == 0) return "-1";
        else
        {
            string temp = resultPage.todayResourceNodeNames[0];
            resultPage.todayResourceNodeNames.RemoveAt(0);
            return temp;
        }
    }
}
