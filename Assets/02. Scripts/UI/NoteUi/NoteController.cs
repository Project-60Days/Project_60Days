using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using DG.Tweening;
using Unity.VisualScripting;

public class NoteController : ControllerBase
{
    [Header("Note Objects")]
    [SerializeField] Text dayText;
    [SerializeField] GameObject noteBackground;

    [Header("Buttons")]
    [SerializeField] Button nextPageBtn;
    [SerializeField] Button prevPageBtn;
    [SerializeField] Button closeBtn;

    public NotePage[] notePages;

    bool isNewDay = true;
    bool isOpen = false;
    int dayCount = 0;
    int pageNum = 0;





    public override EControllerType GetControllerType()
    {
        return EControllerType.NOTE;
    }





    void Start()
    {
        Init();
    }

    void Init()
    {
        ActiveNextBtnAndPrevBtn(false, false);
        ActiveObjects(false);
        InitVariables();
    }

    /// <summary>
    /// 변수 초기화 (첫 날 포함 새로운 날이 시작될 때마다 호출됨)
    /// </summary>
    void InitVariables()
    {
        dayText.text = "Day " + ++dayCount;
        isNewDay = true;
        pageNum = 0;
        notePages = UIManager.instance.GetNextDayController().GetNotePageArray();
    }





    /// <summary>
    /// 노트 열 때 호출
    /// </summary>
    public void OpenNote()
    {
        if (notePages.Length == 0) return;
        if (!isOpen)
        {
            isOpen = true;
            ActiveObjects(true);

            ActiveAndPlayPage();

            if (isNewDay)
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
        if (isOpen)
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
    void ActiveObjects(bool isEnable)
    {
        closeBtn.gameObject.SetActive(isEnable);
        dayText.gameObject.SetActive(isEnable);
        noteBackground.SetActive(isEnable);
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

    public void ChangePageForce(int index) //시연회때문에 임시로 만들었던 함수.. 지우고싶다
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

    public void SetPageNum(ENotePageType type)
    {
        for (int i = 0; i < notePages.Length; i++)
        {
            if (notePages[i].GetENotePageType() == type)
            {
                pageNum = i;
            }
        }
    }
    #endregion





    public void TempOpenBtn() //임시로 ui씬에서 노트를 펼치기 위해 만든 함수
    {
        OpenNote();
    }
}
