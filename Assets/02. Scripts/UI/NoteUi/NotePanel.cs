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
    PageBase[] notePages;

    bool isOpen = false;
    int pageNum = 0;

    [SerializeField] ScrollRect[] scrollRects;
    [SerializeField] Scrollbar[] scrollBars;

    #region Override
    public override void Init()
    {
        SetButtonEvent();
        SetVariables();
        gameObject.SetActive(false);
    }

    public override void ReInit()
    {
        SetVariables();
        ClosePanel(); //TODO
    }

    public override UIState GetUIState() => UIState.Note;

    public override bool IsAddUIStack() => true;

    public override void OpenPanel()
    {
        if (notePages.Length == 0) return;
        if (isOpen) return;

        base.OpenPanel();

        isOpen = !isOpen;

        ActiveAndPlayPage();

        ChangePageButton();

        App.Manager.Sound.PlaySFX("SFX_Note_Open");
    }

    public override void ClosePanel()
    {
        if (!isOpen) return;
        if (App.Manager.UI.CurrState != UIState.Note) return; //TODO

        base.ClosePanel();

        isOpen = !isOpen;

        notePages[pageNum].gameObject.SetActive(false); //TODO

        scrollCtrl.StopAnim(); //TODO

        App.Manager.Sound.PlaySFX("SFX_Note_Close");
    }
    #endregion

    void SetButtonEvent()
    {
        nextPageBtn.onClick.AddListener(() =>
        {
            GoToNextPage();
            App.Manager.Sound.PlaySFX("SFX_Button_1");
        });

        prevPageBtn.onClick.AddListener(() =>
        {
            GoToPrevPage();
            App.Manager.Sound.PlaySFX("SFX_Button_1");
        });

        closeBtn.onClick.AddListener(() =>
        {
            ClosePanel();
        });
    }

    /// <summary>
    /// 변수 초기화 (첫 날 포함 새로운 날이 시작될 때마다 호출됨)
    /// </summary>
    void SetVariables()
    {
        dayText.text = "Day " + App.Manager.Game.DayCount;
        pageNum = 0;
        notePages = GetNotePageArray();
    }

    public PageBase[] GetNotePageArray()
    {
        App.Manager.UI.GetPanel<FixedPanel>().SetAlert(AlertType.Note, false);

        List<PageBase> todayPages = new List<PageBase>();
        foreach (PageBase page in pages)
        {
            page.InitNodeName();

            if (page.GetPageEnableToday() == true)
            {
                todayPages.Add(page);
                App.Manager.UI.GetPanel<FixedPanel>().SetAlert(AlertType.Note, true);
            }

            page.gameObject.SetActive(false);
        }

        return todayPages.ToArray();
    }

    /// <summary>
    /// 페이지 다음/이전 버튼 활성화 또는 비활성화
    /// </summary>
    /// <param name="nextBtnEnable"></param>
    /// <param name="prevBtnEnable"></param>
    void ActiveNextBtnAndPrevBtn(bool _nextBtnEnable, bool _prevBtnEnable)
    {
        if (_nextBtnEnable == false) App.Manager.UI.GetPanel<FixedPanel>().SetAlert(AlertType.Note, false); ;
        nextPageBtn.gameObject.SetActive(_nextBtnEnable);
        prevPageBtn.gameObject.SetActive(_prevBtnEnable);
    }



    /// <summary>
    /// 다음 페이지 버튼 클릭 시 호출
    /// </summary>
    public void GoToNextPage()
    {
        if (notePages[pageNum].CompareIndex() == 2 || notePages[pageNum].CompareIndex() == 1)
        {
            if (pageNum + 1 > notePages.Length - 1)
                return;

            ChangePage(pageNum + 1);
        }
        else
        {
            notePages[pageNum].ChangePageAction("next");
            ChangePageButton();
        }
    }

    /// <summary>
    /// 이전 페이지 버튼 클릭 시 호출
    /// </summary>
    public void GoToPrevPage()
    {
        if (notePages[pageNum].CompareIndex() == 2 || notePages[pageNum].CompareIndex() == -1)
        {
            if (pageNum - 1 < 0)
                return;

            ChangePage(pageNum - 1);
        }
        else
        {
            notePages[pageNum].ChangePageAction("prev");
            ChangePageButton();
        }        
    }

    /// <summary>
    /// 다음/이전 페이지로 이동
    /// </summary>
    /// <param name="index"></param>
    void ChangePage(int _index)
    {
        notePages[pageNum].gameObject.SetActive(false);
        pageNum = _index;
        ActiveAndPlayPage();
        ChangePageButton();
    }

   


    /// <summary>
    /// 새로운 페이지 활성화 및 페이지 동작(Yarn 실행)
    /// </summary>
    void ActiveAndPlayPage()
    {
        notePages[pageNum].gameObject.SetActive(true);
        notePages[pageNum].PlayPageAciton();
    }

    




    /// <summary>
    /// 페이지에 따라 페이지 다음/이전 버튼 활성화 여부 확인
    /// </summary>
    void ChangePageButton()
    {
        StartCoroutine(CheckScrollEnabled());

        if (notePages.Length == 1)
        {
            if (notePages[pageNum].CompareIndex() == 2)
                ActiveNextBtnAndPrevBtn(false, false);
            else if (notePages[pageNum].CompareIndex() == -1)
                ActiveNextBtnAndPrevBtn(true, false);
            else if (notePages[pageNum].CompareIndex() == 1)
                ActiveNextBtnAndPrevBtn(false, true);
            else
                ActiveNextBtnAndPrevBtn(true, true);
        }
        else
        {
            if (pageNum == 0 && (notePages[pageNum].CompareIndex() == -1 || notePages[pageNum].CompareIndex() == 2))
                ActiveNextBtnAndPrevBtn(true, false);
            else if (pageNum == notePages.Length - 1 && (notePages[pageNum].CompareIndex() == 1 || notePages[pageNum].CompareIndex() == 2))
                ActiveNextBtnAndPrevBtn(false, true);
            else
                ActiveNextBtnAndPrevBtn(true, true);
        }
    }

    IEnumerator CheckScrollEnabled()
    {
        scrollCtrl.StopAnim();

        int index;

        if (notePages[pageNum].GetPageType() == PageType.Result)
            index = 0;
        else
            index = 1;

        scrollRects[index].verticalNormalizedPosition = 1.0f;

        yield return null;
        yield return null;

        if (scrollBars[index].gameObject.activeSelf)
        {
            scrollCtrl.StartAnim();
            StartCoroutine(WaitScrollToEnd(scrollBars[index]));
        }
    }

    IEnumerator WaitScrollToEnd(Scrollbar _scroll)
    {
        yield return new WaitUntil(() => _scroll.value <= 0.1f);
        scrollCtrl.StopAnim();
    }





    #region GetAndSet
    public bool CheckIfScrolledToEnd()
    {
        if (scrollBars[1].value <= 0.1f)
            return true;
        else return false;
    }

    public void SetScrollBarInteractable(bool _isInteractable)
    {
        scrollBars[1].enabled = _isInteractable;
        scrollRects[1].enabled = _isInteractable;
    }

    public void SetCloseBtnEnabled(bool _isEnabled)
    {
        closeBtn.enabled = _isEnabled;
    }
    #endregion
}
