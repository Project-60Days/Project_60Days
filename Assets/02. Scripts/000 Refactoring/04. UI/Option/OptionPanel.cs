using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionPanel : UIBase
{
    [SerializeField] Button openBtn;
    [SerializeField] Button closeBtn;

    [SerializeField] Button[] menuBtns;
    [SerializeField] TextMeshProUGUI[] menuBtnTexts;
    [SerializeField] OptionBase[] menuPanels;
    [SerializeField] Sprite[] btnSprites;

    [SerializeField] TextMeshProUGUI soundTMP;
    [SerializeField] TextMeshProUGUI screenTMP;

    #region Override
    public override void Init()
    {
        SetText();
        SetButtonEvent();
    }

    public override void ReInit()
    {
        ClosePanel();
    }

    public override void OpenPanel()
    {
        base.OpenPanel();

        MenuBtnEvent(0);
    }

    public override void ClosePanel()
    {
        base.ClosePanel();

        SaveOptionData();
    }
    #endregion

    private void SetText()
    {
        soundTMP.text = App.Data.Game.GetString("STR_OPTION_SOUND");
        screenTMP.text = App.Data.Game.GetString("STR_OPTION_SCREEN");
    }

    private void SetButtonEvent()
    {
        openBtn?.onClick.AddListener(OpenPanel);
        closeBtn?.onClick.AddListener(ClosePanel);

        for (int i = 0; i < menuBtns.Length; i++)
        {
            int idx = i;

            menuBtns[idx].onClick.AddListener(() => MenuBtnEvent(idx));

            menuPanels[idx].gameObject.SetActive(true);
            menuPanels[idx].gameObject.SetActive(false);
        }
    }

    private void MenuBtnEvent(int _index)
    {
        for (int i = 0; i < menuBtns.Length; i++)
        {
            if (_index == i)
            {
                menuBtns[i].image.sprite = btnSprites[0];
                menuBtnTexts[i].color = Color.black;
                menuPanels[i].gameObject.SetActive(true);
            }
            else
            {
                menuBtns[i].image.sprite = btnSprites[1];
                menuBtnTexts[i].color = Color.white;
                menuPanels[i].gameObject.SetActive(false);
            }
        }
    }

    private void SaveOptionData()
    {
        for (int i = 0; i < menuPanels.Length; i++)
        {
            menuPanels[i].SaveOption();
        }

        App.Data.Setting.SaveToLocal();
    }
}
