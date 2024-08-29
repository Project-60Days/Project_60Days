using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum PageType
{
    Select,
    Result,
    Resource,
}

public class PagePanel : UIBase, IListener
{
    [SerializeField] Button[] selectBtns;

    private Dictionary<PageType, string> tomorrowPageDic = new();

    [SerializeField] Button[] skipButton;

    Color clickedColor = new(56 / 255f, 221 / 255f, 205 / 255f);
    Color unclickedColor = new(1f, 1f, 1f, 0.5f);
    Color normalColor = Color.white;

    private void Awake()
    {
        App.Manager.Event.AddListener(EventCode.TutorialStart, this);
    }

    public void OnEvent(EventCode _code, Component _sender, object _param = null)
    {
        switch (_code)
        {
            case EventCode.TutorialStart:
                SetTutorialSelect();
                break;
        }
    }

    #region Override
    public override void Init() { }

    public override void ReInit()
    {
        ResetBtns();
    }
    #endregion

    public List<string> SetTodayPage()
    {
        List<string> resourceValues = new();
        List<string> sortedValues = new();

        foreach (var item in tomorrowPageDic.OrderBy(x => x.Key))
        {
            if (item.Key == PageType.Resource)
            {
                resourceValues.Add(item.Value);
            }
            else
            {
                sortedValues.Add(item.Value);
            }
        }

        if (resourceValues.Count > 0)
        {
            string combinedResources = string.Join(", ", resourceValues);
            sortedValues.Add(combinedResources);
        }

        tomorrowPageDic.Clear();

        App.Manager.UI.GetPanel<FixedPanel>().SetAlert(AlertType.Note, sortedValues.Count > 0 ? true : false);

        return sortedValues;
    }

    private void ResetBtns()
    {
        foreach (var btn in selectBtns)
        {
            btn.enabled = true;
            btn.image.color = normalColor;
            btn.onClick.RemoveAllListeners();
        }
    }

    public void SetNextPage(PageType _type, string _code, params string[] _param)
    {
        var pageText = string.Format(App.Data.Game.GetString(_code), _param);
        tomorrowPageDic.Add(_type, pageText);
    }

    public void SetNextResourcePage(ItemBase _item, string _code)
    {
        if (_item.Code == "ITEM_NETWORKCHIP")
        {
            SetNextPage(PageType.Result, "STR_PAGE_RESOURCE_NETWORKCHIP");
            return;
        }

        //string tileName = ""; // App.Manager.Map.tileCtrl.Base.GetTileType().ToString();

        int randomNumber = Random.Range(1, 6);

        string nodeName = "STR_RESOURCE_CARBON_JUNGLE5";//  + "_DESERT" + randomNumber.ToString();

        SetNextPage(PageType.Resource, nodeName, "1");
    }

    public void SetNextStructPage(StructBase _struct) // TODO :어떻게 다음날이 됐을 때 이 버튼이 활성화되도록 할 수 ㅣㅇ씅ㄹ까
    {
        SetNextPage(PageType.Select, ""/*_struct.PageCode*/);

        selectBtns[0].onClick.AddListener(_struct.YesFunc);
        selectBtns[1].onClick.AddListener(_struct.NoFunc);
    }

    private void OnClickStructBtn(int _index, StructBase _struct)
    {
        selectBtns[_index].image.color = clickedColor;
        //otherBtn.image.color = unclickedColor;
        selectBtns[_index].enabled = false;
    }

    private void SetTutorialSelect()
    {
        SetNextPage(PageType.Select, "tutorialSelect");

        App.Manager.UI.GetPanel<NotePanel>().ReInit();

        ResetBtns();

        selectBtns[0].onClick.AddListener(() =>
        {
            App.Manager.UI.GetPanel<InventoryPanel>().RemoveItemByCode("ITEM_BATTERY");
            App.Manager.UI.GetPanel<NotePanel>().ClosePanel();
        });
    }
}
