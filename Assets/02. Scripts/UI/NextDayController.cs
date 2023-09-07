using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NextDayController : ControllerBase
{
    [SerializeField] Image blackPanel;
    public NotePage[] pages;

    [Header("Quest Objects")]
    [SerializeField] GameObject questPrefab;
    [SerializeField] Transform questParent;

    [Header("Alarm Objects")]
    [SerializeField] GameObject newAlarm;
    [SerializeField] GameObject resultAlarm;
    [SerializeField] GameObject cautionAlarm;

    CanvasGroup shelterUi;

    CinemachineVirtualCamera mapCamera; //임시

    public override EControllerType GetControllerType()
    {
        return EControllerType.NEXTDAY;
    }





    void Awake()
    {
        Init();
        pages = GameObject.Find("Page_Back").GetComponentsInChildren<NotePage>(includeInactive: true);
    }

    void Start()
    {
        shelterUi = GameObject.FindGameObjectWithTag("ShelterUi").GetComponent<CanvasGroup>();
        mapCamera = GameObject.FindGameObjectWithTag("MapCamera").GetComponent<CinemachineVirtualCamera>();//임시
    }




    /// <summary>
    /// 온갖 초기화 함수 모음
    /// </summary>
    void Init()
    {
        InitBlackPanel();
        InitPageEnabled();
        InitQuestList();
        InitAlarm();
    }

    #region Inits
    /// <summary>
    /// 활성화 됐던 BlackPanel 다시 비홣성화 (BlackPanel은 다음 날로 넘어갈 때 잠깐 보이는 까만 화면)
    /// </summary>
    void InitBlackPanel()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(blackPanel.DOFade(0f, 1f))
            .OnComplete(() => blackPanel.gameObject.SetActive(false));
        sequence.Play();   
    }

    /// <summary>
    /// 노트 페이지 초기화 (dialogueRunner 멈추기, 페이지 비활성화) --> 추가 작업 필요. 데이터 연결해야함.
    /// </summary>
    void InitPageEnabled()
    {
        foreach (NotePage page in pages)
        {
            page.StopDialogue();
            page.gameObject.SetActive(false);
            //이후 새로운 날이 시작될 때마다 데이터 받아와 page.SetPageEnabled() 호출하여 값 넘겨주기
        }
    }

    /// <summary>
    /// 퀘스트 목록 초기화 (생성되었던 프리팹 모두 파괴)
    /// </summary>
    void InitQuestList()
    {
        Quest[] quests = questParent.GetComponentsInChildren<Quest>();
        foreach (Quest quest in quests)
        {
            Destroy(quest.gameObject);
        }
    }

    /// <summary>
    /// 알림 목록 초기화 --> 현재는 필요한 알림만 SetActive(true)해서 사용 중인데, 더 좋은 방법을 고민 중. / 마찬가지로 추가 작업 필요. 데이터 연결해야함.
    /// </summary>
    void InitAlarm()
    {
        newAlarm.SetActive(false);
        resultAlarm.SetActive(false);
        cautionAlarm.SetActive(false);
    }
    #endregion





    /// <summary>
    /// 다음 날이 될 때 BlackPanel 활성화/페이드인
    /// </summary>
    public void NextDayEvent()
    {
        blackPanel.gameObject.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(blackPanel.DOFade(1f, 0.5f)).SetEase(Ease.InQuint)
            .AppendInterval(0.5f)
            .Append(shelterUi.DOFade(1f, 0f))
            .OnComplete(() => NextDayEventCallBack());
        sequence.Play();
    }

    /// <summary>
    /// 바로 위의 NextDayEvent()의 콜백함수 (초기화 작업)
    /// </summary>
    void NextDayEventCallBack()
    {
        Init();
        App.instance.GetMapManager().SetMapCameraPriority(false);

        UIManager.instance.GetNoteController().SetNextDay();
        App.instance.GetMapManager().AllowMouseEvent(true);
        MapController.instance.NextDay();
    }


    public void BackToShelter() //임시..........
    {
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(shelterUi.DOFade(1f, 0.5f));
        sequence.Play();
    }

    public void ZoomOutMap() //임시.........................
    {
        StartCoroutine("OrthoAnim");
    }

    IEnumerator OrthoAnim() //임시..................................
    {
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.05f);
            mapCamera.m_Lens.OrthographicSize += 0.05f;
        }
        App.instance.GetMapManager().SetMapCameraPriority(false);
        BackToShelter();
    }


    #region PageSetting
    /// <summary>
    /// 새로운 날에 쓰이는 페이지만 모아 NoteController에 배열로 전달. (page.GetPageEnableToday()함수로 사용 여부 확인)
    /// </summary>
    /// <returns></returns>
    public NotePage[] GetNotePageArray()
    {
        List<NotePage> todayPages = new List<NotePage>();
        foreach (NotePage page in pages)
        {
            if (page.GetPageEnableToday())
            {
                todayPages.Add(page);
            }
        }

        return todayPages.ToArray(); ;
    }
    #endregion





    #region QuestSetting
    /// <summary>
    /// 퀘스트 프리팹 추가 --> 추가 작업 필요.. 그리고 좀 더 간결하게 함수 나누고 싶음.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="text"></param>
    void AddQuest(EQuestType type, string text)
    {
        GameObject obj = Instantiate(questPrefab, questParent);
        Quest quest = obj.GetComponent<Quest>();
        quest.SetEQuestType(type);
        quest.SetText(text);
        quest.SetQuestTypeText();
        quest.SetQuestTypeImage();
        SetQuestList();
    }

    /// <summary>
    /// 퀘스트 리스트 정렬(?) 메인퀘스트는 위로, 서브퀘스트는 아래에 뜨게. --> 위의 AddQuest함수 정리 되면 마찬가지로 얘도 손보고 싶음. 더 좋은 방법이 있을듯!
    /// </summary>
    void SetQuestList()
    {
        Quest[] quests = questParent.GetComponentsInChildren<Quest>();
        foreach (Quest quest in quests)
        {
            if(quest.GetEQuestType() == EQuestType.Main)
                quest.transform.SetAsFirstSibling();
            else
                quest.transform.SetAsLastSibling();
        }
    }
    #endregion





    #region ForTest
    public void AddMainQuestBtn() //테스트용 임시 함수. 메인퀘스트 추가 버튼
    {
        AddQuest(EQuestType.Main, "예시입니다");
    }

    public void AddSubQuestBtn() //테스트용 임시 함수. 서브퀘스트 추가 버튼
    {
        AddQuest(EQuestType.Sub, "예시여");
    }
    public void AddResultPage() //테스트용 임시 함수. 다음 날에 결과 페이지 활성화 버튼
    {
        pages[0].SetPageEnabled(true);
    }
    public void RemoveResultPage() //테스트용 임시 함수. 다음 날에 결과 페이지 비활성화 버튼
    {
        pages[0].SetPageEnabled(false);
    }
    public void AddSelectPage() //테스트용 임시 함수. 다음 날에 선택 페이지 활성화 버튼
    {
        pages[1].SetPageEnabled(true);
    }
    public void RemoveSelectPage() //테스트용 임시 함수. 다음 날에 선택 페이지 비활성화 버튼
    {
        pages[1].SetPageEnabled(false);
    }
    #endregion





    //아무튼 전체적으로 새로운 날 될 때 전 날 데이터 확인해야 하는 것들은 전부 추가 작업 필요함(퀘스트, 노트 페이지, 알림)
}