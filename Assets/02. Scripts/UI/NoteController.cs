using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using DG.Tweening;
using Unity.VisualScripting;

public class NoteController : ControllerBase
{
    [Header("Note Pos")]
    [SerializeField] RectTransform noteCenterPos;
    [SerializeField] RectTransform noteRightPos;
    [SerializeField] RectTransform notePos;

    [Header("Note Objects")]
    [SerializeField] Image[] noteBackgrounds;
    [SerializeField] Text dayText;
    [SerializeField] GameObject noteBackground_Back;
    [SerializeField] GameObject inventoryUi;

    [Header("Buttons")]
    [SerializeField] Button nextPageBtn;
    [SerializeField] Button prevPageBtn;
    [SerializeField] Button closeBtn;

    [Header("Tutorial Diary")]
    [SerializeField] GameObject page_Diary_Back;
    [SerializeField] DialogueRunner diaryDialogue;
    [SerializeField] VerticalLayoutGroup diaryContent;
    [SerializeField] VerticalLayoutGroup diaryLineView;
    int DiaryPageNum = 1;

    NotePage[] notePages;

    bool isTutorial = false;
    bool isNewDay = true;
    bool isOpen = false;
    int dayCount = 1;

    int pageNum = 0;

    SetNextDay setNextDay;

    public override EControllerType GetControllerType()
    {
        return EControllerType.NOTE;
    }

    public void Start()
    {
        Init();
    }

    void Init()
    {
        setNextDay = GameObject.Find("NextDay_Btn").GetComponent<SetNextDay>();
        notePages = setNextDay.GetNotePageArray();

        DisableObjectsInInit();
    }

    void DisableObjectsInInit()
    {
        //foreach (var image in noteBackgrounds)
        //    image.DOFade(0f, 0f);

        ActiveNextBtnAndPrevBtn(false, false);

        closeBtn.gameObject.SetActive(false);
        dayText.gameObject.SetActive(false);
        noteBackground_Back.SetActive(false);

        inventoryUi.SetActive(false);

        foreach(var page in notePages)
            page.gameObject.SetActive(false);
    }

    private void MoveNoteCenter()
    {
        notePos.DOAnchorPos(new Vector2(noteCenterPos.anchoredPosition.x, notePos.anchoredPosition.y), 1f);
    }

    private void MoveNoteRight()
    {
        notePos.DOAnchorPos(new Vector2(noteRightPos.anchoredPosition.x, notePos.anchoredPosition.y), 1f);
    }

    public void OpenNote()
    {
        if(!isOpen)
        {
            noteBackground_Back.SetActive(true);
            OpenNoteCallBack(); //

            //DOTween.Kill(gameObject);

            //Sequence sequence = DOTween.Sequence();
            //sequence.AppendCallback(() =>
            //{
            //    foreach (var notePanel in noteBackgrounds)
            //        notePanel.DOFade(1f, 1f);
            //})
            //    .AppendInterval(1f)
            //    .OnComplete(() => OpenNoteCallBack());

            //sequence.Play(); 
        }
    }

    public void OpenNoteCallBack()
    {
        isOpen = true;
        dayText.gameObject.SetActive(true);
        closeBtn.gameObject.SetActive(true);

        EnableAndPlayPage();

        if (isNewDay)
            isNewDay = false;

        ChangePageButton();

        //if (isTutorial)
        //{
        //    page_Diary_Back.SetActive(true);
        //    DiaryPageNum = 1;
        //    LoadDiaryPage(DiaryPageNum);
        //}
        //else
        //{
            
        //}
        
        UIManager.instance.AddCurrUIName(StringUtility.UI_NOTE);
    }

    public void CloseNote()
    {
        if (isOpen)
        {
            DisAbleObjectsInClose();

            MoveNoteCenter();
            CloseNoteCallBack(); //
            //DOTween.Kill(gameObject);

            //Sequence sequence = DOTween.Sequence();
            //sequence.AppendCallback(() =>
            //{
            //    foreach (var notePanel in noteBackgrounds)
            //        notePanel.DOFade(0f, 0.5f);
            //})
            //    .AppendInterval(0.5f)
            //    .OnComplete(() => CloseNoteCallBack());

            //sequence.Play();
        }
    }

    void CloseNoteCallBack()
    {
        isOpen = false;
        noteBackground_Back.SetActive(false);

        UIManager.instance.PopCurrUI();
    }

    void DisAbleObjectsInClose()
    {
        notePages[pageNum].gameObject.SetActive(false);
        dayText.gameObject.SetActive(false);
        inventoryUi.gameObject.SetActive(false);
        closeBtn.gameObject.SetActive(false);

        ActiveNextBtnAndPrevBtn(false, false);
    }

    public void SetNextDay()
    {
        dayText.text = "Day " + ++dayCount;
        isNewDay = true;
        pageNum = 0;
        notePages = setNextDay.GetNotePageArray();
        for (int i = 0; i < notePages.Length; i++)
            notePages[i].gameObject.SetActive(false);
    }

    public void NextPageEvent()
    {
        if (pageNum + 1 > notePages.Length - 1)
            return;

        ChangePage(pageNum + 1);
    }

    /// <summary>
    /// 이전 페이지 버튼 클릭 시 호출
    /// </summary>
    public void PrevPageEvent()
    {
        if (pageNum - 1 < 0)
            return;

        ChangePage(pageNum - 1);
    }

    /// <summary>
    /// 다음/이전 페이지로 이동
    /// </summary>
    /// <param name="index"></param>
    void ChangePage(int index)
    {
        notePages[pageNum].gameObject.SetActive(false);
        pageNum = index;
        EnableAndPlayPage();
        ChangePageButton();
    }

    void EnableAndPlayPage()
    {
        notePages[pageNum].gameObject.SetActive(true);
        notePages[pageNum].PlayPageAction();
        SetNotePos();
    }

    void SetNotePos()
    {
        if (notePages[pageNum].GetENotePageType() == ENotePageType.CraftEquipment)
            MoveNoteRight();
        else
            MoveNoteCenter();
    }

    public void ChangePageForce(int index)
    {
        pageNum = index;

        Debug.Log("실행");
        if (!isOpen)
        {
            OpenNote();
            Debug.Log("열림");
        }


        for (int i = 0; i < notePages.Length; i++)
        {
            notePages[i].gameObject.SetActive(false);
            Debug.Log(i + "닫힘");
        }

        notePages[index].gameObject.SetActive(true);
        Debug.Log(index);
        notePages[index].PlayPageAction();
        ChangePageButton();
    }

    /// <summary>
    /// 페이지 이동 버튼 이미지 변경
    /// </summary>
    void ChangePageButton()
    {
        if (pageNum == 0)
            ActiveNextBtnAndPrevBtn(true, false);
        else if (pageNum == notePages.Length - 1)
            ActiveNextBtnAndPrevBtn(false, true);
        else
            ActiveNextBtnAndPrevBtn(true, true);
    }

    void ActiveNextBtnAndPrevBtn(bool nextBtnEnable, bool prevBtnEnable)
    {
        nextPageBtn.gameObject.SetActive(nextBtnEnable);
        prevPageBtn.gameObject.SetActive(prevBtnEnable);
    }

    #region GetAndSet
    public bool GetIsOpen()
    {
        return isOpen;
    }

    public bool GetNewDay()
    {
        return isNewDay;
    }

    public int GetDayCount()
    {
        return dayCount;
    }

    public int GetPageNum(ENotePageType type)
    {
        for (int i = 0; i < notePages.Length; i++) 
        {
            if (notePages[i].GetENotePageType() == type)
            {
                return i;
            }
        }

        return -1;
    }

    public void SetPageNum(int index) 
    {
        pageNum = index;
    }
    #endregion

    #region TutorialDiary
    public void SetTutorialDiary()
    {
        Debug.Log("SetTutorialDiary");
        isTutorial = true;

        nextPageBtn.onClick.RemoveAllListeners();
        prevPageBtn.onClick.RemoveAllListeners();

        nextPageBtn.onClick.AddListener(() => { DiaryPageNum++; LoadDiaryPage(DiaryPageNum); });
        prevPageBtn.onClick.AddListener(() => { DiaryPageNum--; LoadDiaryPage(DiaryPageNum); });
    }

    public void LoadDiaryPage(int _idx)
    {
        if (_idx < 1)
            return;

        if (_idx == 5)
        {
            EndTutorialDiary();
            return;
        }

        if (_idx == 1)
            prevPageBtn.gameObject.SetActive(false);
        else
            prevPageBtn.gameObject.SetActive(true);

        string nodeName = "Diary_Page_" + _idx.ToString();

        diaryDialogue.Stop();
        diaryDialogue.StartDialogue(nodeName);
        LayoutRebuilder.ForceRebuildLayoutImmediate(diaryContent.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(diaryLineView.GetComponent<RectTransform>());

    }

    /// <summary>
    /// 일기 읽기 튜토리얼 종료
    /// </summary>
    public void EndTutorialDiary()
    {
        isTutorial = false;
        page_Diary_Back.SetActive(false);
        CloseNote();
        SetBtnNormal();
        TutorialManager.instance.tutorialController.SetNextTutorial();
    }

    private void SetBtnNormal()
    {
        nextPageBtn.onClick.RemoveAllListeners();
        prevPageBtn.onClick.RemoveAllListeners();

        nextPageBtn.onClick.AddListener(NextPageEvent);
        prevPageBtn.onClick.AddListener(PrevPageEvent);
    }
    #endregion
}
