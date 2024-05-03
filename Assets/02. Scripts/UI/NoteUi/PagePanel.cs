using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class PagePanel : UIBase
{
    [SerializeField] Button yesBtn;
    [SerializeField] Button noBtn;

    Image yesImage;
    Image noImage;

    [SerializeField] GameObject resultPrefab;
    [SerializeField] RectTransform resultParent;
    [SerializeField] GameObject selectPrefab;
    [SerializeField] RectTransform selectParent;

    PageBase resultPage;
    PageBase selectPage;

    [SerializeField] Button[] skipButton;

    Color clickedColor = new Color(56 / 255f, 221 / 255f, 205 / 255f);
    Color unclickedColor = new Color(1f, 1f, 1f, 0.5f);
    Color normalColor = new Color(1f, 1f, 1f, 1f);

    [HideInInspector] public string currStruct;
    [HideInInspector] public string currResource;
    [HideInInspector] public int currResourceIndex = 0;
    [HideInInspector] public bool isClickYesBtnInTower = false;
    [HideInInspector] public bool isClickYesBtnInProductionStructure = false;

    #region Override
    public override void Init()
    {
        PageBase[] pages = GetComponentsInChildren<PageBase>(includeInactive: true);
        foreach (var page in pages)
        {
            if (page.GetPageType() == PageType.Result)
                resultPage = page;
            else if (page.GetPageType() == PageType.Select)
                selectPage = page;
        }

        yesImage = yesBtn.GetComponent<Image>();
        noImage = noBtn.GetComponent<Image>();

        currStruct = null;
        isClickYesBtnInTower = false;
        isClickYesBtnInProductionStructure = false;
    }

    public override void ReInit()
    {
        var resources = App.Manager.Map.resourceCtrl.GetLastResources();

        for (int i = 0; i < resources.Count; i++)
        {
            string tileName = App.Manager.Map.mapCtrl
                .Player.TileController.GetComponent<TileBase>().TileData.English;

            int randomNumber = Random.Range(1, 6);

            string nodeName = resources[i].Item.Code + "_" + tileName + randomNumber.ToString();

            if (resources[i].Item.Code == "ITEM_NETWORKCHIP")
                SetResultPage(nodeName, false);
            else
                SetResultPage(nodeName, true);
        }
    }
    #endregion

    public void SetResultPage(string _nodeName, bool _isResourceNode)
    {
        resultPage.SetNodeName(_nodeName, _isResourceNode);
    }

    public void SetSelectPage(string _nodeName, StructureBase _structData)
    {
        selectPage.SetNodeName(_nodeName);
        currStruct = _structData.name;

        InitBtns();

        yesBtn.onClick.AddListener(_structData.YesFunc);
        noBtn.onClick.AddListener(_structData.NoFunc);

        AddDefaultListener();
    }

    void InitBtns()
    {
        yesBtn.enabled = true;
        noBtn.enabled = true;

        yesImage.color = normalColor;
        noImage.color = normalColor;

        yesBtn.onClick.RemoveAllListeners();
        noBtn.onClick.RemoveAllListeners();
    }

    void AddDefaultListener()
    {
        yesBtn.onClick.AddListener(SetYesBtnColored);
        noBtn.onClick.AddListener(SetNoBtnColored);

        yesBtn.onClick.AddListener(SetBtnsEnbled);
        noBtn.onClick.AddListener(SetBtnsEnbled);
    }

    void SetYesBtnColored()
    {
        yesImage.color = clickedColor;
        noImage.color = unclickedColor;
    }

    void SetNoBtnColored()
    {
        yesImage.color = unclickedColor;
        noImage.color = clickedColor;
    }

    void SetBtnsEnbled()
    {
        yesBtn.enabled = false;
        noBtn.enabled = false;
    }

    public void SetTutorialSelect()
    {
        selectPage.SetNodeName("tutorialSelect");
        App.Manager.Game.dayCount = -1;
        App.Manager.UI.GetPanel<NotePanel>().ReInit();

        yesBtn.onClick.RemoveAllListeners();
        noBtn.onClick.RemoveAllListeners();

        yesBtn.onClick.AddListener(TutorialYesFunc);
    }

    public void TutorialYesFunc()
    {
        //TutorialManager.instance.GetTutorialController().LightUpBackground();
        App.Manager.UI.GetPanel<InventoryPanel>().RemoveItemByCode("ITEM_BATTERY");
        App.Manager.UI.GetPanel<NotePanel>().ClosePanel();
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

    public void CreateResultDialogueRunner(string _nodeName)
    {
        GameObject obj = Instantiate(resultPrefab, resultParent);

        DialogueRunner dialogueRunner = obj.GetComponent<DialogueRunner>();

        dialogueRunner.StartDialogue(_nodeName);

        LayoutRebuilder.ForceRebuildLayoutImmediate(resultParent);
    }

    public void CreateSelectDialogueRunner(string _nodeName)
    {
        GameObject obj = Instantiate(selectPrefab, selectParent);

        obj.GetComponent<CustomDialogueView>().skipButton = skipButton;

        DialogueRunner dialogueRunner = obj.GetComponent<DialogueRunner>();

        dialogueRunner.StartDialogue(_nodeName);

        LayoutRebuilder.ForceRebuildLayoutImmediate(selectParent);
    }

    public void SetCurrResource(ItemBase _item)
    {
        currResource = _item.data.Korean;

        switch (_item.data.Code)
        {
            case "ITEM_STEEL":
                currResourceIndex = 0;
                break;
            case "ITEM_CARBON":
                currResourceIndex = 1;
                break;
            case "ITEM_PLASMA":
                currResourceIndex = 2;
                break;
            case "ITEM_POWDER":
                currResourceIndex = 4;
                break;
            case "ITEM_GAS":
                currResourceIndex = 5;
                break;
            case "ITEM_RUBBER":
                currResourceIndex = 6;
                break;
            default:
                currResourceIndex = 3;
                break;
        }
    }
}
