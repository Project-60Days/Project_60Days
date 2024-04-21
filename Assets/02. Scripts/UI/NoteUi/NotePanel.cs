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

    [SerializeField] NotePageBase[] pages;
    NotePageBase[] notePages;

    [HideInInspector] public bool isNewDay = true;
    [HideInInspector] bool isOpen = false;
    [HideInInspector] public int dayCount = 0;
    [HideInInspector] int pageNum = 0;

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

        if (isNewDay)
            isNewDay = !isNewDay;

        ChangePageButton();

        App.Manager.Sound.PlaySFX("SFX_Note_Open");
    }

    public override void ClosePanel()
    {
        if (!isOpen) return;
        if (!App.Manager.UI.isUIStatus(UIState.Note)) return; //TODO

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
            App.Manager.Sound.PlaySFX("SFX_ButtonClick_01");
        });

        prevPageBtn.onClick.AddListener(() =>
        {
            GoToPrevPage();
            App.Manager.Sound.PlaySFX("SFX_ButtonClick_01");
        });

        closeBtn.onClick.AddListener(() =>
        {
            ClosePanel();
        });
    }

    /// <summary>
    /// ���� �ʱ�ȭ (ù �� ���� ���ο� ���� ���۵� ������ ȣ���)
    /// </summary>
    void SetVariables()
    {
        dayText.text = "Day " + ++dayCount;
        pageNum = 0;
        notePages = GetNotePageArray();
    }

    public NotePageBase[] GetNotePageArray()
    {
        List<NotePageBase> todayPages = new List<NotePageBase>();
        foreach (NotePageBase page in pages)
        {
            page.InitNodeName();

            if (page.GetPageEnableToday() == true)
            {
                todayPages.Add(page);
                App.Manager.UI.GetPanel<AlertPanel>().SetAlert("note", true);
            }

            page.gameObject.SetActive(false);
        }

        return todayPages.ToArray();
    }

    /// <summary>
    /// ������ ����/���� ��ư Ȱ��ȭ �Ǵ� ��Ȱ��ȭ
    /// </summary>
    /// <param name="nextBtnEnable"></param>
    /// <param name="prevBtnEnable"></param>
    void ActiveNextBtnAndPrevBtn(bool _nextBtnEnable, bool _prevBtnEnable)
    {
        if (_nextBtnEnable == false) App.Manager.UI.GetPanel<AlertPanel>().SetAlert("note", false);
        nextPageBtn.gameObject.SetActive(_nextBtnEnable);
        prevPageBtn.gameObject.SetActive(_prevBtnEnable);
    }



    /// <summary>
    /// ���� ������ ��ư Ŭ�� �� ȣ��
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
    /// ���� ������ ��ư Ŭ�� �� ȣ��
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
    /// ����/���� �������� �̵�
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
    /// ���ο� ������ Ȱ��ȭ �� ������ ����(Yarn ����)
    /// </summary>
    void ActiveAndPlayPage()
    {
        notePages[pageNum].gameObject.SetActive(true);
        notePages[pageNum].PlayPageAciton();
    }

    




    /// <summary>
    /// �������� ���� ������ ����/���� ��ư Ȱ��ȭ ���� Ȯ��
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

        if (notePages[pageNum].GetENotePageType() == ENotePageType.Result)
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
    public bool GetNewDay()
    {
        return isNewDay;
    }

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
