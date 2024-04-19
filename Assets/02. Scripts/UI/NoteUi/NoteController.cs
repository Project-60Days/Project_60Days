using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class NoteController : MonoBehaviour
{
    [Header("Note Objects")]
    [SerializeField] Text dayText;
    [SerializeField] GameObject noteBackground;
    [SerializeField] NoteScroll scrollImg;

    [Header("Buttons")]
    [SerializeField] Button nextPageBtn;
    [SerializeField] Button prevPageBtn;
    [SerializeField] Button closeBtn;

    [SerializeField] NotePageBase[] pages;
    NotePageBase[] notePages;

    [HideInInspector] public bool isNewDay = true;
    bool isOpen = false;
    [HideInInspector] public int dayCount = 0;
    int pageNum = 0;

    [SerializeField] ScrollRect[] scrollRects;
    [SerializeField] Scrollbar[] scrollBars;



    void Start()
    {
        //pages = GetComponentsInChildren<NotePageBase>(includeInactive: true);

        Init();
    }

    void Init()
    {
        ActiveNextBtnAndPrevBtn(false, false);
        ActiveObjects(false);
        InitVariables();
    }

    /// <summary>
    /// ������ ����/���� ��ư Ȱ��ȭ �Ǵ� ��Ȱ��ȭ
    /// </summary>
    /// <param name="nextBtnEnable"></param>
    /// <param name="prevBtnEnable"></param>
    void ActiveNextBtnAndPrevBtn(bool _nextBtnEnable, bool _prevBtnEnable)
    {
        if (_nextBtnEnable == false) UIManager.instance.GetAlertController().SetAlert("note", false);
        nextPageBtn.gameObject.SetActive(_nextBtnEnable);
        prevPageBtn.gameObject.SetActive(_prevBtnEnable);
    }

    /// <summary>
    /// ��Ʈ ���� ���� �� ��ġ�� ������Ʈ Ȱ��ȭ/��Ȱ��ȭ
    /// </summary>
    /// <param name="isEnable"></param>
    void ActiveObjects(bool _isEnable)
    {
        closeBtn.gameObject.SetActive(_isEnable);
        dayText.gameObject.SetActive(_isEnable);
        noteBackground.SetActive(_isEnable);
    }

    /// <summary>
    /// ���� �ʱ�ȭ (ù �� ���� ���ο� ���� ���۵� ������ ȣ���)
    /// </summary>
    void InitVariables()
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
                UIManager.instance.GetAlertController().SetAlert("note", true);
            }

            page.gameObject.SetActive(false);
        }

        return todayPages.ToArray();
    }





    /// <summary>
    /// ��Ʈ �� �� ȣ��
    /// </summary>
    public void OpenNote()
    {
        if (notePages.Length == 0) return;
        if (isOpen == false)
        {
            isOpen = true;
            ActiveObjects(true);

            ActiveAndPlayPage();

            if (isNewDay == true)
                isNewDay = false;

            ChangePageButton();

            App.Manager.Sound.PlaySFX("SFX_Note_Open");
            UIManager.instance.AddCurrUIName(StringUtility.UI_NOTE);
        }
    }
    
    /// <summary>
    /// ��Ʈ ���� �� ȣ��
    /// </summary>
    public void CloseNote()
    {
        if (isOpen == true)
        {
            if (UIManager.instance.isUIStatus("UI_NOTE"))
                UIManager.instance.PopCurrUI();
            else return;

            isOpen = false;
            ActiveObjects(false);

            ActiveNextBtnAndPrevBtn(false, false);

            notePages[pageNum].gameObject.SetActive(false);

            scrollImg.StopAnim();

            App.Manager.Sound.PlaySFX("SFX_Note_Close");   
        }
    }





    /// <summary>
    /// ���� ���� �Ǿ��� �� ȣ���. ���� �ʱ�ȭ �� ��Ʈ ����
    /// </summary>
    public void SetNextDay()
    {
        InitVariables();
        CloseNote();
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
        scrollImg.StopAnim();

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
            scrollImg.StartAnim();
            StartCoroutine(WaitScrollToEnd(scrollBars[index]));
        }
    }

    IEnumerator WaitScrollToEnd(Scrollbar _scroll)
    {
        yield return new WaitUntil(() => _scroll.value <= 0.1f);
        scrollImg.StopAnim();
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
