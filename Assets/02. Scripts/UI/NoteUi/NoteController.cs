using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NoteController : ControllerBase
{
    [Header("Note Objects")]
    [SerializeField] Text dayText;
    [SerializeField] GameObject noteBackground;

    [Header("Buttons")]
    [SerializeField] Button nextPageBtn;
    [SerializeField] Button prevPageBtn;
    [SerializeField] Button closeBtn;

    public NotePageBase[] pages;
    NotePageBase[] notePages;

    bool isNewDay = true;
    bool isOpen = false;
    int dayCount = 0;
    int pageNum = 0;





    public override EControllerType GetControllerType()
    {
        return EControllerType.NOTE;
    }





    void Awake()
    {
        pages = GetComponentsInChildren<NotePageBase>(includeInactive: true);

        Init();
    }

    void Init()
    {
        ActiveNextBtnAndPrevBtn(false, false);
        ActiveObjects(false);
        InitVariables();
        InitPageEnabled();
    }

    /// <summary>
    /// 변수 초기화 (첫 날 포함 새로운 날이 시작될 때마다 호출됨)
    /// </summary>
    void InitVariables()
    {
        dayText.text = "Day " + ++dayCount;
        isNewDay = true;
        pageNum = 0;
        notePages = GetNotePageArray();
    }

    /// <summary>
    /// 노트 페이지 초기화
    /// </summary>
    void InitPageEnabled()
    {
        foreach (NotePageBase page in pages)
        {
            page.StopDialogue();
            page.gameObject.SetActive(false);
            //page.SetPageEnabled()로 다음 날 사용할 페이지 결정
        }
    }




    #region PageSetting
    /// <summary>
    /// 다음 날로 넘어갈 때 노트 페이지 구성 함수
    /// </summary>
    /// <returns></returns>
    public void SetDiary(string _code, StructData _structData)
    {
        var nextDiaryData = App.instance.GetDataManager().diaryData[_code];
        //var nextDiary += .script;
        SetNote("result", true);
        if (nextDiaryData.IsSelectScript == 0)
        {
            //yes += Invoke(_structData.YesFunctionName);
            //yes += SetDiary(_structData.code += "_Enter");
            //no += Invoke(_structData.NoFunction);
            //no += SetDiary(_structData.code += "_Pass");
        }
    }

    public NotePageBase[] GetNotePageArray()
    {
        List<NotePageBase> todayPages = new List<NotePageBase>();
        foreach (NotePageBase page in pages)
        {
            if (page.GetPageEnableToday())
                todayPages.Add(page);
        }

        return todayPages.ToArray();
    }

    public void SetNote(string _noteType, bool _isActive)
    {
        if (_noteType == "result")
            pages[0].SetPageEnabled(_isActive);
        else if (_noteType == "select")
            pages[1].SetPageEnabled(_isActive);
    }
    #endregion




    /// <summary>
    /// 노트 열 때 호출
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

            App.instance.GetSoundManager().PlaySFX("SFX_Note_Open");
            UIManager.instance.AddCurrUIName(StringUtility.UI_NOTE);
        }
    }
    
    /// <summary>
    /// 노트 닫을 때 호출
    /// </summary>
    public void CloseNote()
    {
        if (isOpen == true)
        {
            isOpen = false;
            ActiveObjects(false);
            ActiveNextBtnAndPrevBtn(false, false);

            notePages[pageNum].gameObject.SetActive(false);

            App.instance.GetSoundManager().PlaySFX("SFX_Note_Close");
            UIManager.instance.PopCurrUI();
        }
    }

    /// <summary>
    /// 노트 열고 닫을 때 겹치는 오브젝트 활성화/비활성화
    /// </summary>
    /// <param name="isEnable"></param>
    void ActiveObjects(bool _isEnable)
    {
        closeBtn.gameObject.SetActive(_isEnable);
        dayText.gameObject.SetActive(_isEnable);
        noteBackground.SetActive(_isEnable);
    }

    public void OnAfterSelect()
    {
        notePages[pageNum].gameObject.SetActive(false);

        CloseNote();
    }




    /// <summary>
    /// 다음 날이 되었을 때 호출됨. 변수 초기화 및 노트 닫음
    /// </summary>
    public void SetNextDay()
    {
        InitVariables();
        CloseNote();
    }





    /// <summary>
    /// 다음 페이지 버튼 클릭 시 호출
    /// </summary>
    public void GoToNextPage()
    {
        if (pageNum + 1 > notePages.Length - 1)
            return;

        ChangePage(pageNum + 1);
    }

    /// <summary>
    /// 이전 페이지 버튼 클릭 시 호출
    /// </summary>
    public void GoToPrevPage()
    {
        if (pageNum - 1 < 0)
            return;

        ChangePage(pageNum - 1);
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
        notePages[pageNum].PlayPageAction();
    }





    /// <summary>
    /// 페이지에 따라 페이지 다음/이전 버튼 활성화 여부 확인
    /// </summary>
    void ChangePageButton()
    {
        if (notePages.Length == 1)
            ActiveNextBtnAndPrevBtn(false, false);
        else
        {
            if (pageNum == 0)
                ActiveNextBtnAndPrevBtn(true, false);
            else if (pageNum == notePages.Length - 1)
                ActiveNextBtnAndPrevBtn(false, true);
            else
                ActiveNextBtnAndPrevBtn(true, true);
        }
    }

    /// <summary>
    /// 페이지 다음/이전 버튼 활성화 또는 비활성화
    /// </summary>
    /// <param name="nextBtnEnable"></param>
    /// <param name="prevBtnEnable"></param>
    void ActiveNextBtnAndPrevBtn(bool _nextBtnEnable, bool _prevBtnEnable)
    {
        nextPageBtn.gameObject.SetActive(_nextBtnEnable);
        prevPageBtn.gameObject.SetActive(_prevBtnEnable);
    }





    #region GetAndSet
    public bool GetNewDay()
    {
        return isNewDay;
    }
    #endregion
}
