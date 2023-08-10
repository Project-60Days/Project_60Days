using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;
using UnityEditor.Search;
using Unity.VisualScripting.Antlr3.Runtime;

public class NoteController : MonoBehaviour
{
    [SerializeField] RectTransform noteCenterPos;
    [SerializeField] RectTransform noteRightPos;
    [SerializeField] RectTransform notePos;
    [SerializeField] GameObject pageContainer;
    [SerializeField] Transform[] notePages;
    [SerializeField] GameObject inventory;

    [SerializeField] Button nextPageBtn;
    [SerializeField] Button prevPageBtn;
    [SerializeField] Button nextDayBtn;

    [SerializeField] DialogueRunner[] dialogueRunner;

    public bool newDay = true;
    int dialogueRunnerIndex = 0;
    string nodeName;

    bool isTutorial = false;
    [SerializeField] GameObject page_Diary_Back;
    [SerializeField] DialogueRunner diaryDialogue;
    [SerializeField] VerticalLayoutGroup diaryContent;
    [SerializeField] VerticalLayoutGroup diaryLineView;
    int DiaryPageNum = 1;

    public int pageNum = 0;
    //int dayCount = 1;

    //List<int> numbers = new List<int>() { 1, 2, 3, 4, 5 };
    //int selectedNumber;

    [SerializeField] public NoteAnim noteAnim;

    [SerializeField] VerticalLayoutGroup[] contents;
    [SerializeField] VerticalLayoutGroup[] lineViews;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
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

        //CameraMove cameraMove = FindObjectOfType<CameraMove>();

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

        inventory.SetActive(false);

    }

    private void MoveNoteCenter()
    {
        notePos.DOAnchorPos(new Vector2(noteCenterPos.anchoredPosition.x, notePos.anchoredPosition.y), 1f);
    }

    private void MoveNoteRight()
    {
        notePos.DOAnchorPos(new Vector2(noteRightPos.anchoredPosition.x, notePos.anchoredPosition.y), 1f);
    }

    /// <summary>
    /// 상자 열릴 때 NoteAnim.cs에서 호출되는 함수
    /// </summary>
    public void OpenBox()
    {
        if (isTutorial)
        {
            page_Diary_Back.SetActive(true);
            DiaryPageNum = 1;
            LoadDiaryPage(DiaryPageNum);
        }
        else
        {
            notePages[pageNum].gameObject.SetActive(true);
            if (newDay)
            {
                PageOn(0);
                newDay = false;
            }
            else
                PageOn(pageNum);

            ChangePageButton();
        }

    }
    /// <summary>
    /// 상자 닫힐 때 NoteAnim.cs에서 호출되는 함수 
    /// </summary>
    public void CloseBox()
    {
        notePages[pageNum].gameObject.SetActive(false);
        nextPageBtn.gameObject.SetActive(false);
        prevPageBtn.gameObject.SetActive(false);

        notePos.DOAnchorPos(new Vector2(noteCenterPos.anchoredPosition.x, notePos.anchoredPosition.y), 1f);
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

    public void ChangePageForce(int index)
    {
        pageNum = index;

        Debug.Log("실행");
        if (!noteAnim.GetIsOpen())
        {
            noteAnim.Open_Anim();
            Debug.Log("열림");
        }
            

        for (int i = 0; i < notePages.Length; i++) 
        {
            notePages[i].gameObject.SetActive(false);
            Debug.Log(i+"닫힘");
        }

        notePages[index].gameObject.SetActive(true);
        Debug.Log(index);
        PageOn(index);
        ChangePageButton();
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
    /// <summary>
    /// 한 페이지 내에서 yarn node 이동
    /// </summary>
    /// <param name="index"></param>
    void PageOn(int index)
    {
        switch (index)
        {
            case 0:
                var pos = inventory.transform.position;
                pos.x = 450;
                inventory.transform.position = pos;
                inventory.SetActive(true);
                GameManager.instance.SetPrioryty(false);
                Debug.Log("인덱스 0");
                MoveNoteRight();
                break;
            case 1:
                inventory.SetActive(false);
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

        if (!dialogueRunner[dialogueRunnerIndex].IsDialogueRunning)
        {
            dialogueRunner[dialogueRunnerIndex].StartDialogue(nodeName);
            LayoutRebuilder.ForceRebuildLayoutImmediate(contents[dialogueRunnerIndex].GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(lineViews[dialogueRunnerIndex].GetComponent<RectTransform>());
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
        else if (pageNum == 1) //pageNum == notePages.Length - 1
        {
            nextPageBtn.gameObject.SetActive(false);
            prevPageBtn.gameObject.SetActive(true);
            nextDayBtn.gameObject.SetActive(true);
        }
        //else
        //{
        //    nextPageBtn.gameObject.SetActive(true);
        //    prevPageBtn.gameObject.SetActive(true);
        //    nextDayBtn.gameObject.SetActive(false);
        //}
    }

    private void SetBtnNormal()
    {
        nextPageBtn.onClick.RemoveAllListeners();
        prevPageBtn.onClick.RemoveAllListeners();

        nextPageBtn.onClick.AddListener(NextPageEvent);
        prevPageBtn.onClick.AddListener(PrevPageEvent);
        nextDayBtn.onClick.AddListener(NextDayEvent);
    }

    /// <summary>
    /// 제출 버튼 클릭 시 일과 노트 내용 초기화
    /// </summary>
    void NextDayEvent()
    {
        App.instance.GetMapManager().NextDay();
        pageNum = 0;
        for (int i = 0; i < dialogueRunner.Length; i++)
            dialogueRunner[i].Stop();
        //int randomIndex = UnityEngine.Random.Range(0, numbers.Count);
        //selectedNumber = numbers[randomIndex];
        //numbers.RemoveAt(randomIndex);
        GameManager.instance.SetPrioryty(false);
        //dayCount++;
    }

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
        noteAnim.Close_Anim();
        SetBtnNormal();
        TutorialManager.instance.tutorialController.SetNextTutorial();
    }

    public bool GetNewDay()
    {
        return newDay;
    }
}

