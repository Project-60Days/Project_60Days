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

    CinemachineVirtualCamera mapCamera;
    CinemachineFramingTransposer transposer;

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
        mapCamera = GameObject.FindGameObjectWithTag("MapCamera").GetComponent<CinemachineVirtualCamera>();
        transposer = mapCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        App.instance.AddController(this);
    }




    /// <summary>
    /// �°� �ʱ�ȭ �Լ� ����
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
    /// Ȱ��ȭ �ƴ� BlackPanel �ٽ� ���b��ȭ (BlackPanel�� ���� ���� �Ѿ �� ��� ���̴� � ȭ��)
    /// </summary>
    void InitBlackPanel()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(blackPanel.DOFade(0f, 1f))
            .OnComplete(() => blackPanel.gameObject.SetActive(false));
        sequence.Play();   
    }

    /// <summary>
    /// ��Ʈ ������ �ʱ�ȭ (dialogueRunner ���߱�, ������ ��Ȱ��ȭ) --> �߰� �۾� �ʿ�. ������ �����ؾ���.
    /// </summary>
    void InitPageEnabled()
    {
        foreach (NotePage page in pages)
        {
            page.StopDialogue();
            page.gameObject.SetActive(false);
            //���� ���ο� ���� ���۵� ������ ������ �޾ƿ� page.SetPageEnabled() ȣ���Ͽ� �� �Ѱ��ֱ�
        }
    }

    /// <summary>
    /// ����Ʈ ��� �ʱ�ȭ (�����Ǿ��� ������ ��� �ı�)
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
    /// �˸� ��� �ʱ�ȭ --> ����� �ʿ��� �˸��� SetActive(true)�ؼ� ��� ���ε�, �� ���� ����� ���� ��. / ���������� �߰� �۾� �ʿ�. ������ �����ؾ���.
    /// </summary>
    void InitAlarm()
    {
        newAlarm.SetActive(false);
        resultAlarm.SetActive(false);
        cautionAlarm.SetActive(false);
    }
    #endregion





    /// <summary>
    /// ���� ���� �� �� BlackPanel Ȱ��ȭ/���̵���
    /// </summary>
    public void NextDayEvent()
    {
        blackPanel.gameObject.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(blackPanel.DOFade(1f, 0.5f)).SetEase(Ease.InQuint)
            .AppendInterval(0.5f)
            .Append(shelterUi.DOFade(1f, 0f))
            .OnComplete(() => NextDayEventCallBack());
        //sequence.Play();
    }

    /// <summary>
    /// �ٷ� ���� NextDayEvent()�� �ݹ��Լ� (�ʱ�ȭ �۾�)
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

    public void ZoomOutMap()
    {
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(DOTween.To(() => transposer.m_CameraDistance, x => transposer.m_CameraDistance = x, 15f, 0.5f))
            .OnComplete(() =>
            {
                App.instance.GetMapManager().SetMapCameraPriority(false);
            })
            .Append(shelterUi.DOFade(1f, 0.5f));
        sequence.Play();
    }





    #region PageSetting
    /// <summary>
    /// ���ο� ���� ���̴� �������� ��� NoteController�� �迭�� ����. (page.GetPageEnableToday()�Լ��� ��� ���� Ȯ��)
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
    /// ����Ʈ ������ �߰� --> �߰� �۾� �ʿ�.. �׸��� �� �� �����ϰ� �Լ� ������ ����.
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
    /// ����Ʈ ����Ʈ ����(?) ��������Ʈ�� ����, ��������Ʈ�� �Ʒ��� �߰�. --> ���� AddQuest�Լ� ���� �Ǹ� ���������� �굵 �պ��� ����. �� ���� ����� ������!
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
    public void AddMainQuestBtn() //�׽�Ʈ�� �ӽ� �Լ�. ��������Ʈ �߰� ��ư
    {
        AddQuest(EQuestType.Main, "�����Դϴ�");
    }

    public void AddSubQuestBtn() //�׽�Ʈ�� �ӽ� �Լ�. ��������Ʈ �߰� ��ư
    {
        AddQuest(EQuestType.Sub, "���ÿ�");
    }
    public void AddResultPage() //�׽�Ʈ�� �ӽ� �Լ�. ���� ���� ��� ������ Ȱ��ȭ ��ư
    {
        pages[0].SetPageEnabled(true);
    }
    public void RemoveResultPage() //�׽�Ʈ�� �ӽ� �Լ�. ���� ���� ��� ������ ��Ȱ��ȭ ��ư
    {
        pages[0].SetPageEnabled(false);
    }
    public void AddSelectPage() //�׽�Ʈ�� �ӽ� �Լ�. ���� ���� ���� ������ Ȱ��ȭ ��ư
    {
        pages[1].SetPageEnabled(true);
    }
    public void RemoveSelectPage() //�׽�Ʈ�� �ӽ� �Լ�. ���� ���� ���� ������ ��Ȱ��ȭ ��ư
    {
        pages[1].SetPageEnabled(false);
    }

    public void AddResultAlarm()
    {
        resultAlarm.SetActive(true);
        //resultAlarm.GetComponent<Alarm>().AddResult();
    }
    public void AddCautionAlarm()
    {
        cautionAlarm.SetActive(true);
        //cautionAlarm.GetComponent<Alarm>().AddCaution();
    }
    #endregion





    //�ƹ�ư ��ü������ ���ο� �� �� �� �� �� ������ Ȯ���ؾ� �ϴ� �͵��� ���� �߰� �۾� �ʿ���(����Ʈ, ��Ʈ ������, �˸�)
}