using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NotePanel : UIBase
{
    [Header("Note Objects")]
    [SerializeField] Text dayText;
    [SerializeField] GameObject noteBackground;
    [SerializeField] NoteScrollCtrl scrollCtrl;

    [Header("Buttons")]
    [SerializeField] Button nextPageBtn;
    [SerializeField] Button prevPageBtn;
    [SerializeField] Button closeBtn;

    [SerializeField] PageBase[] pages;
    private PageBase[] notePages;

    private bool isOpen = false;
    private int pageNum = 0;

    [SerializeField] ScrollRect[] scrollRects;
    [SerializeField] Scrollbar[] scrollBars;

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
    }

    public override void OpenPanel()
    {
        if (notePages.Length == 0 || isOpen) return;

        base.OpenPanel();
        isOpen = true;

        ActivateCurrentPage();
        UpdatePageButtons();

        App.Manager.Sound.PlaySFX("SFX_Note_Open");
    }

    public override void ClosePanel()
    {
        if (!isOpen || App.Manager.UI.CurrState != UIState.Note) return;

        base.ClosePanel();
        isOpen = false;

        notePages[pageNum].gameObject.SetActive(false);
        scrollCtrl.StopAnim();

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
        notePages = GetActiveNotePages();
    }

    private PageBase[] GetActiveNotePages()
    {
        App.Manager.UI.GetPanel<FixedPanel>().SetAlert(AlertType.Note, false);

        List<PageBase> todayPages = new();

        foreach (var page in pages)
        {
            page.InitNodeName();

            if (page.GetPageEnableToday())
            {
                todayPages.Add(page);
                App.Manager.UI.GetPanel<FixedPanel>().SetAlert(AlertType.Note, true);
            }

            page.gameObject.SetActive(false);
        }

        return todayPages.ToArray();
    }

    private void NavigatePage(int direction)
    {
        var currentPage = notePages[pageNum];
        int compareIndex = currentPage.CompareIndex();

        if ((direction == 1 && (compareIndex == 1 || compareIndex == 2)) ||
            (direction == -1 && (compareIndex == -1 || compareIndex == 2)))
        {
            int newIndex = pageNum + direction;
            if (newIndex >= 0 && newIndex < notePages.Length)
            {
                ChangePage(newIndex);
            }
        }
        else
        {
            currentPage.ChangePageAction(direction == 1 ? "next" : "prev");
            UpdatePageButtons();
        }
    }

    /// <summary>
    /// 다음/이전 페이지로 이동
    /// </summary>
    /// <param name="index"></param>
    private void ChangePage(int newIndex)
    {
        notePages[pageNum].gameObject.SetActive(false);
        pageNum = newIndex;
        ActivateCurrentPage();
        UpdatePageButtons();
    }

    private void ActivateCurrentPage()
    {
        notePages[pageNum].gameObject.SetActive(true);
        notePages[pageNum].PlayPageAciton();
    }

    private void UpdatePageButtons()
    {
        StartCoroutine(CheckScrollEnabled());

        bool nextBtnEnabled = pageNum < notePages.Length - 1 || notePages[pageNum].CompareIndex() != 1;
        bool prevBtnEnabled = pageNum > 0 || notePages[pageNum].CompareIndex() != -1;

        if (notePages.Length == 1)
        {
            prevBtnEnabled = nextBtnEnabled = false;
        }

        SetNavigationButtons(nextBtnEnabled, prevBtnEnabled);
    }

    private void SetNavigationButtons(bool nextBtnEnabled, bool prevBtnEnabled)
    {
        nextPageBtn.gameObject.SetActive(nextBtnEnabled);
        prevPageBtn.gameObject.SetActive(prevBtnEnabled);

        if (!nextBtnEnabled)
        {
            App.Manager.UI.GetPanel<FixedPanel>().SetAlert(AlertType.Note, false);
        }
    }

    private IEnumerator CheckScrollEnabled()
    {
        scrollCtrl.StopAnim();

        int index = notePages[pageNum].GetPageType() == PageType.Result ? 0 : 1;
        scrollRects[index].verticalNormalizedPosition = 1.0f;

        yield return null;

        if (scrollBars[index].gameObject.activeSelf)
        {
            scrollCtrl.StartAnim();
            StartCoroutine(WaitScrollToEnd(scrollBars[index]));
        }
    }

    private IEnumerator WaitScrollToEnd(Scrollbar scrollBar)
    {
        yield return new WaitUntil(() => scrollBar.value <= 0.1f);
        scrollCtrl.StopAnim();
    }

    #region GetAndSet
    public bool CheckIfScrolledToEnd() => scrollBars[1].value <= 0.1f;

    public void SetScrollBarInteractable(bool isInteractable)
    {
        scrollBars[1].enabled = isInteractable;
        scrollRects[1].enabled = isInteractable;
    }

    public void SetCloseBtnEnabled(bool isEnabled)
    {
        closeBtn.enabled = isEnabled;
    }
    #endregion
}
