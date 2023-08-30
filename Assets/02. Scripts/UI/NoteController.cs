using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using DG.Tweening;

public class NoteController : ControllerBase
{
    [Header("Note Pos")]
    [SerializeField] RectTransform noteCenterPos;
    [SerializeField] RectTransform noteRightPos;
    [SerializeField] RectTransform notePos;

    [Header("Note Pages")]
    [SerializeField] GameObject pageContainer;
    [SerializeField] Transform[] notePages;

    [Header("Note Objects")]
    [SerializeField] Image[] notePanels;
    [SerializeField] Image blackPanel;
    [SerializeField] Text dayText;
    [SerializeField] GameObject noteBackground_Back;
    [SerializeField] GameObject inventoryUi;

    [Header("Buttons")]
    [SerializeField] Button nextPageBtn;
    [SerializeField] Button prevPageBtn;
    [SerializeField] Button nextDayBtn;

    [Header("Tutorial Diary")]
    [SerializeField] GameObject page_Diary_Back;
    [SerializeField] DialogueRunner diaryDialogue;
    [SerializeField] VerticalLayoutGroup diaryContent;
    [SerializeField] VerticalLayoutGroup diaryLineView;
    int DiaryPageNum = 1;

    bool isTutorial = false;
    bool isNewDay = true;
    bool isOpen = false;
    int dayCount = 1;

    int pageNum = 0;
    
    public override EControllerType GetControllerType()
    {
        return EControllerType.NOTE;
    }

    public void Start()
    {
        nextDayBtn.onClick.AddListener(NextDayEvent);

        Init();
    }

    void Init()
    {
        foreach (var notePanel in notePanels)
            notePanel.DOFade(0f, 0f);

        blackPanel.DOFade(0f, 0f);
        blackPanel.gameObject.SetActive(false);

        nextPageBtn.gameObject.SetActive(false);
        prevPageBtn.gameObject.SetActive(false);
        nextDayBtn.gameObject.SetActive(false);

        dayText.gameObject.SetActive(false);
        noteBackground_Back.SetActive(false);

        inventoryUi.SetActive(false);

        Transform[] pages = pageContainer.GetComponentsInChildren<Transform>();
        List<Transform> targets = new List<Transform>();
        foreach (Transform page in pages)
        {
            if (page.CompareTag("NotePage"))
            {
                targets.Add(page);
            }
        }

        notePages = targets.ToArray();

        for (int i = 0; i < notePages.Length; i++)
        {
            notePages[i].gameObject.SetActive(false);
            //var page = notePages[i].GetComponent<NotePage>();
            //page.Init(cameraMove);

            //if (page.isNoteMoveRight)
            //    page.pageOnEvent += MoveNoteRight;
            //else
            //    page.pageOnEvent += MoveNoteCenter;
        }

        //int randomIndex = UnityEngine.Random.Range(0, numbers.Count);
        //selectedNumber = numbers[randomIndex];
        //numbers.RemoveAt(randomIndex);
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

            DOTween.Kill(gameObject);

            Sequence sequence = DOTween.Sequence();
            sequence.AppendCallback(() =>
            {
                foreach (var notePanel in notePanels)
                    notePanel.DOFade(1f, 0.5f);
            })
                .AppendInterval(0.5f)
                .OnComplete(() => OpenNoteCallBack());

            sequence.Play(); 
        }
    }

    public void OpenNoteCallBack()
    {
        isOpen = true;
        dayText.gameObject.SetActive(true);

        if (isTutorial)
        {
            page_Diary_Back.SetActive(true);
            DiaryPageNum = 1;
            LoadDiaryPage(DiaryPageNum);
        }
        else
        {
            notePages[pageNum].gameObject.SetActive(true);
            if (isNewDay)
            {
                PageOn(0);
                isNewDay = false;
            }
            else
                PageOn(pageNum);

            ChangePageButton();
        }
        
        UIManager.instance.AddCurrUIName(StringUtility.UI_NOTE);
    }

    public void CloseNote()
    {
        if (isOpen)
        {
            notePages[pageNum].gameObject.SetActive(false);
            nextPageBtn.gameObject.SetActive(false);
            prevPageBtn.gameObject.SetActive(false);

            MoveNoteCenter();
            dayText.gameObject.SetActive(false);

            DOTween.Kill(gameObject);

            Sequence sequence = DOTween.Sequence();
            sequence.AppendCallback(() =>
            {
                foreach (var notePanel in notePanels)
                    notePanel.DOFade(0f, 0.5f);
            })
                .AppendInterval(0.5f)
                .OnComplete(() => CloseNoteCallBack());

            inventoryUi.gameObject.SetActive(false);
            sequence.Play();
        }
    }

    void CloseNoteCallBack()
    {
        isOpen = false;
        noteBackground_Back.SetActive(false);

        UIManager.instance.PopCurrUI();
    }

    void NextDayEvent()
    {
        CloseNote();
        blackPanel.gameObject.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(blackPanel.DOFade(1f, 0.5f)).SetEase(Ease.InQuint)
            .AppendInterval(0.5f)
            .Append(blackPanel.DOFade(0f, 0.5f + 0.5f))
            .OnComplete(() => NextDayEventCallBack());
        sequence.Play();

    }

    void NextDayEventCallBack()
    {
        blackPanel.gameObject.SetActive(false);
        dayText.text = "Day " + ++dayCount;
        isNewDay = true;
        App.instance.GetMapManager().AllowMouseEvent(true);
        MapController.instance.NextDay();
        pageNum = 0;
        GameManager.instance.SetPrioryty(false);
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
    public void ChangePage(int index)
    {
        notePages[pageNum].gameObject.SetActive(false);
        notePages[index].gameObject.SetActive(true);

        PageOn(index);
        pageNum = index;
        ChangePageButton();
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
        PageOn(index);
        ChangePageButton();
    }


    /// <summary>
    /// 한 페이지 내에서 yarn node 이동
    /// </summary>
    /// <param name="index"></param>
    void PageOn(int index)
    {
        switch (index)
        {
            case 0:
                var pos = inventoryUi.transform.position;
                pos.x = 450;
                inventoryUi.transform.position = pos;
                inventoryUi.SetActive(true);
                GameManager.instance.SetPrioryty(false);
                Debug.Log("인덱스 0");
                MoveNoteRight();
                break;
            case 1:
                inventoryUi.SetActive(false);
                GameManager.instance.SetPrioryty(true);
                Debug.Log("인덱스 1");
                MoveNoteCenter();
                break;
            default:
                Debug.Log("인덱스 범위 벗어남");
                return;
                //case 0:
                //    dialogueRunnerIndex = 0;
                //    nodeName = "Day" + dayCount;
                //    inventory.SetActive(false);
                //    break;
                //case 1:
                //    dialogueRunnerIndex = 1;
                //    nodeName = "Day" + dayCount + "ChooseEvent";
                //    inventory.SetActive(false);
                //    break;
                //case 2:
                //    dialogueRunnerIndex = 2;
                //    nodeName = "d" + selectedNumber;
                //    inventory.SetActive(false);
                //    break;
                //case 3:
                //    var pos = inventory.transform.position;
                //    pos.x = 450;
                //    inventory.transform.position = pos;
                //    inventory.SetActive(true);
                //    break;
                //case 4:
                //    GameManager.instance.SetPrioryty(false);
                //    inventory.SetActive(false);
                //    break;
                //case 5:
                //    GameManager.instance.SetPrioryty(true);
                //    break;
                ////    noteAnim.Close_Anim();
                ////    return;
                //default:
                //    return;
        }
    }

    /// <summary>
    /// 페이지 이동 버튼 이미지 변경
    /// </summary>
    void ChangePageButton()
    {
        if (pageNum == 0)
        {
            nextPageBtn.gameObject.SetActive(true);
            prevPageBtn.gameObject.SetActive(false);
            nextDayBtn.gameObject.SetActive(false);
        }
        else if (pageNum == notePages.Length - 1)
        {
            nextPageBtn.gameObject.SetActive(false);
            prevPageBtn.gameObject.SetActive(true);
            nextDayBtn.gameObject.SetActive(true);
        }
        else
        {
            nextPageBtn.gameObject.SetActive(true);
            prevPageBtn.gameObject.SetActive(true);
            nextDayBtn.gameObject.SetActive(false);
        }
    }

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


    /// <summary>
    /// 아래는 튜토리얼관련 함수들
    /// </summary>


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
        nextDayBtn.onClick.AddListener(NextDayEvent);
    }
}
