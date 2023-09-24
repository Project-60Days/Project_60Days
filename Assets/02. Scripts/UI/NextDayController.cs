using Cinemachine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextDayController : ControllerBase
{
    [SerializeField] Image blackPanel;
    public NotePageBase[] pages;

    [Header("Quest Objects")]
    [SerializeField] GameObject questPrefab;
    [SerializeField] Transform questParent;
    [SerializeField] GameObject questLogo;

    [Header("Alert Objects")]
    [SerializeField] GameObject selectAlert;
    [SerializeField] GameObject cautionAlert;

    CanvasGroup shelterUi;

    CinemachineVirtualCamera mapCamera;
    CinemachineFramingTransposer transposer;

    public override EControllerType GetControllerType()
    {
        return EControllerType.NEXTDAY;
    }





    void Awake()
    {
        Init();
        pages = GameObject.Find("Page_Back").GetComponentsInChildren<NotePageBase>(includeInactive: true);
    }

    void Start()
    {
        shelterUi = GameObject.FindGameObjectWithTag("ShelterUi").GetComponent<CanvasGroup>();
        mapCamera = GameObject.FindGameObjectWithTag("MapCamera").GetComponent<CinemachineVirtualCamera>();
        transposer = mapCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        transposer.m_CameraDistance = 5f;
        App.instance.AddController(this); //?
    }




    /// <summary>
    /// 초기화 함수 모음
    /// </summary>
    void Init()
    {
        InitBlackPanel();
        InitPageEnabled();
        InitQuestList();
        InitAlert();
    }

    #region Inits
    /// <summary>
    /// BlackPanel 초기화 --> BlackPanel: 다음 날로 넘어갈 때 깜빡거리는 용도
    /// </summary>
    void InitBlackPanel()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(blackPanel.DOFade(0f, 1f))
            .OnComplete(() => blackPanel.gameObject.SetActive(false));
        sequence.Play();   
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

    /// <summary>
    /// 퀘스트 리스트 초기화
    /// </summary>
    void InitQuestList()
    {
        Quest[] quests = questParent.GetComponentsInChildren<Quest>();
        foreach (Quest quest in quests)
            Destroy(quest.gameObject);
    }

    /// <summary>
    /// 알림 리스트 초기화
    /// </summary>
    void InitAlert()
    {
        selectAlert.SetActive(false);
        cautionAlert.SetActive(false);
    }
    #endregion





    /// <summary>
    /// 다음 날로 넘어갈 때 호출되는 이벤트 --> blackPanel 깜빡거림
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
    /// NextDayEvent 콜백함수
    /// </summary>
    void NextDayEventCallBack()
    {
        Init();
        App.instance.GetMapManager().SetMapCameraPriority(false);
        transposer.m_CameraDistance = 15f;
        UIManager.instance.GetNoteController().SetNextDay();
        App.instance.GetMapManager().AllowMouseEvent(true);
        StartCoroutine(App.instance.GetMapManager().NextDayCoroutine());
    }

    public void GoToLab()
    {
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(DOTween.To(() => transposer.m_CameraDistance, x => transposer.m_CameraDistance = x, 5f, 0.5f))
            .OnComplete(() => App.instance.GetMapManager().SetMapCameraPriority(false))
            .Append(shelterUi.DOFade(1f, 0.5f));
        sequence.Play();
    }

    public void GoToMap()
    {
        blackPanel.gameObject.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(shelterUi.DOFade(0f, 0.5f))
            .Join(blackPanel.DOFade(1f, 0.5f))
            .OnComplete(() => ZoomInMap());
        sequence.Play();
    }

    void ZoomInMap()
    {
        App.instance.GetMapManager().SetMapCameraPriority(true);
        blackPanel.DOFade(0f, 0.5f);
        DOTween.To(() => transposer.m_CameraDistance, x => transposer.m_CameraDistance = x, 10f, 0.5f)
            .OnComplete(()=> blackPanel.gameObject.SetActive(false));
    }



    #region PageSetting
    /// <summary>
    /// 다음 날로 넘어갈 때 노트 페이지 구성 함수
    /// </summary>
    /// <returns></returns>
    public NotePageBase[] GetNotePageArray()
    {
        List<NotePageBase> todayPages = new List<NotePageBase>();
        foreach (NotePageBase page in pages)
        {
            if (page.GetPageEnableToday())
                todayPages.Add(page);
        }

        return todayPages.ToArray(); ;
    }

    public void SetNote(string _noteType, bool _isActive)
    {
        if (_noteType == "result")
            pages[0].SetPageEnabled(_isActive);
        else if (_noteType == "select")
            pages[1].SetPageEnabled(_isActive);
    }
    #endregion





    #region QuestSetting
    /// <summary>
    /// 다음 날로 넘어갈 때 퀘스트 리스트 구성 함수
    /// </summary>
    /// <param name="type"></param>
    /// <param name="text"></param>
    void AddQuest(EQuestType _type)
    {
        GameObject obj = Instantiate(questPrefab, questParent);
        Quest quest = obj.GetComponent<Quest>();
        quest.SetEQuestType(_type);
        quest.SetQuestTypeText();
        quest.SetQuestTypeImage();
        SetQuestList();
    }

    /// <summary>
    /// 퀘스트 리스트 순서 정렬 함수 (임시로 메인퀘스트는 위에, 서브퀘스트는 아래에 위치하게 설정)
    /// </summary>
    void SetQuestList()
    {
        Quest[] quests = questParent.GetComponentsInChildren<Quest>();

        if (quests.Length > 0)
            questLogo.SetActive(true);

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
    public void AddMainQuestBtn() //�׽�Ʈ�� �ӽ� �Լ�. ��������Ʈ �߰� ��ư
    {
        AddQuest(EQuestType.Main);
    }

    public void AddSubQuestBtn() //�׽�Ʈ�� �ӽ� �Լ�. ��������Ʈ �߰� ��ư
    {
        AddQuest(EQuestType.Sub);
    }
   
    public void AddResultAlarm()
    {
        selectAlert.SetActive(true);
        //resultAlarm.GetComponent<Alarm>().AddResult();
    }
    public void AddCautionAlarm()
    {
        cautionAlert.SetActive(true);
        //cautionAlarm.GetComponent<Alarm>().AddCaution();
    }
    #endregion
}