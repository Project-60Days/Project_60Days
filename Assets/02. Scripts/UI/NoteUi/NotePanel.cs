using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

public class NotePanel : UIBase
{
    [Header("Note Objects")]
    [SerializeField] Text dayText;
    [SerializeField] NoteScrollCtrl scrollCtrl;

    [Header("Buttons")]
    [SerializeField] Button nextPageBtn;
    [SerializeField] Button prevPageBtn;
    [SerializeField] Button closeBtn;
    [SerializeField] TextMeshProUGUI pageText;

    private int pageNum = 0;

    [SerializeField] ScrollRect scrollRect;
    [SerializeField] Scrollbar scrollBar;

    private string[] todayPage;

    #region Override
    public override UIState GetUIState() => UIState.Note;

    public override bool IsAddUIStack() => true;

    public override void Init()
    {
        SetButtonEvent();
        SetVariables();
        gameObject.SetActive(false);
    }

    public override void ReInit()
    {
        SetVariables();

        App.Manager.UI.GetPanel<FixedPanel>().SetAlert(AlertType.Note, todayPage.Length > 0 ? true : false);
    }

    public override void OpenPanel()
    {
        if (todayPage.Length == 0 || App.Manager.UI.CurrState != UIState.Normal) return;

        base.OpenPanel();

        ActivateCurrentPage();
        UpdatePageButtons();

        App.Manager.Sound.PlaySFX("SFX_Note_Open");
    }

    public override void ClosePanel()
    {
        if (App.Manager.UI.CurrState != UIState.Note) return;

        base.ClosePanel();

        App.Manager.Sound.PlaySFX("SFX_Note_Close");
    }
    #endregion

    private void SetButtonEvent()
    {
        nextPageBtn.onClick.AddListener(() => NavigatePage(1));
        prevPageBtn.onClick.AddListener(() => NavigatePage(-1));
        closeBtn.onClick.AddListener(ClosePanel);
    }

    /// <summary>
    /// 변수 초기화 (첫 날 포함 새로운 날이 시작될 때마다 호출됨)
    /// </summary>
    private void SetVariables()
    {
        dayText.text = $"Day {App.Manager.Game.DayCount}";
        pageNum = 0;
        todayPage = App.Manager.UI.GetPanel<PagePanel>().SetTodayPage();
    }

    private void NavigatePage(int direction)
    {
        pageNum += direction;
        pageNum = Mathf.Clamp(pageNum, 0, todayPage.Length - 1);

        ChangePage();
    }

    /// <summary>
    /// 다음/이전 페이지로 이동
    /// </summary>
    /// <param name="index"></param>
    private void ChangePage()
    {
        ActivateCurrentPage();
        UpdatePageButtons();
    }

    private void ActivateCurrentPage()
    {
        pageText.DOKill();

        pageText.DOFade(0f, 0.05f).OnComplete(() =>
        {
            pageText.text = todayPage[pageNum];
            scrollCtrl.ResetScrollActive();
            pageText.DOFade(1f, 0.05f);
        });
    }

    private void UpdatePageButtons()
    {
        bool nextBtnEnabled = pageNum < todayPage.Length - 2;
        bool prevBtnEnabled = pageNum > 0;

        nextPageBtn.gameObject.SetActive(nextBtnEnabled);
        prevPageBtn.gameObject.SetActive(prevBtnEnabled);

        if (!nextBtnEnabled)
        {
            App.Manager.UI.GetPanel<FixedPanel>().SetAlert(AlertType.Note, false);
        }
    }

    #region GetAndSet
    public bool CheckIfScrolledToEnd() => scrollBar.value <= 0.1f;

    public void SetScrollBarInteractable(bool isInteractable)
    {
        scrollBar.enabled = isInteractable;
        scrollRect.enabled = isInteractable;
    }

    public void SetCloseBtnEnabled(bool isEnabled)
    {
        closeBtn.enabled = isEnabled;
    }
    #endregion
}
