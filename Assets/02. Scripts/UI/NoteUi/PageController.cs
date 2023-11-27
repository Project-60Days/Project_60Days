using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class PageController : MonoBehaviour
{
    [SerializeField] Button yesBtn;
    [SerializeField] Button noBtn;

    [SerializeField] GameObject resultPrefab;
    [SerializeField] RectTransform resultParent;

    NotePageBase resultPage;
    NotePageBase selectPage;

    Color clickedColor = new Color(56 / 255f, 221 / 255f, 205 / 255f);

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

        yesBtn.enabled = true;
        noBtn.enabled = true;

        yesBtn.onClick.RemoveAllListeners();
        noBtn.onClick.RemoveAllListeners();

        yesBtn.onClick.AddListener(_structData.YesFunc);
        noBtn.onClick.AddListener(_structData.NoFunc);

        AddDefaultListener();
    }

    void AddDefaultListener()
    {
        yesBtn.onClick.AddListener(UIManager.instance.GetNoteController().CloseNote);
        noBtn.onClick.AddListener(UIManager.instance.GetNoteController().CloseNote);

        yesBtn.onClick.AddListener(SetYesBtnColored);
        noBtn.onClick.AddListener(SetNoBtnColored);

        yesBtn.onClick.AddListener(SetBtnsEnbled);
        noBtn.onClick.AddListener(SetBtnsEnbled);
    }

    void SetYesBtnColored()
    {
        yesBtn.GetComponent<Image>().color = clickedColor;
    }

    void SetNoBtnColored()
    {
        noBtn.GetComponent<Image>().color = clickedColor;
    }

    void SetBtnsEnbled()
    {
        yesBtn.enabled = false;
        noBtn.enabled = false;
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
            resultPage.resourceIndex++;
            if (resultPage.resourceIndex > resultPage.todayResourceNodeNames.Count - 1) return "-1";
            string temp = resultPage.todayResourceNodeNames[resultPage.resourceIndex];
            return temp;
        }
    }

    public void CreateDialogueRunner(string _nodeName)
    {
        GameObject obj = Instantiate(resultPrefab, resultParent);

        DialogueRunner dialogueRunner = obj.GetComponent<DialogueRunner>();

        dialogueRunner.StartDialogue(_nodeName);

        LayoutRebuilder.ForceRebuildLayoutImmediate(resultParent);
    }
}
