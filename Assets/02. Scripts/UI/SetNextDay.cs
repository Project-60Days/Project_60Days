using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SetNextDay : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] Image blackPanel;
    
    [SerializeField] NotePage[] pages;

    [Header("Gauage Obejcts")]
    [SerializeField] Image gaugeImage;
    [SerializeField] float fillSpeed = 1.0f;
    [SerializeField] float maxGaugeValue = 100.0f;
    float currentGaugeValue = 0.0f;
    bool isFilling = false;

    [Header("Quest Objects")]
    [SerializeField] GameObject questPrefab;
    [SerializeField] Transform questParent;

    [SerializeField] GameObject NewAlarm;
    [SerializeField] GameObject ResultAlarm;
    [SerializeField] GameObject CautionAlarm;

    Quest[] quests;

    void Start()
    {
        Init();
        gaugeImage.fillAmount = 0;
    }

    void Init()
    {
        blackPanel.DOFade(0f, 0f);
        blackPanel.gameObject.SetActive(false);
        InitPageEnabled();
    }

    void Update()
    {
        if (isFilling)
            FillGauge();
        if (currentGaugeValue >= maxGaugeValue)
        {
            InitGauageUI();
            NextDayEvent();
        }    
    }
    void NextDayEvent()
    {
        UIManager.instance.GetNoteController().CloseNote();
        blackPanel.gameObject.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(blackPanel.DOFade(1f, 0.5f)).SetEase(Ease.InQuint)
            .AppendInterval(0.5f)
            .OnComplete(() => NextDayEventCallBack());
        sequence.Play();

        Debug.Log("2");
    }

    void InitQuestAndAlarm()
    {
        InitQuestList();
        NewAlarm.SetActive(false);
        ResultAlarm.SetActive(false);
        CautionAlarm.SetActive(false);

    }

    void NextDayEventCallBack()
    {
        InitQuestAndAlarm();
        blackPanel.DOFade(0f, 0.5f + 0.5f);
        blackPanel.gameObject.SetActive(false);
        UIManager.instance.GetNoteController().SetNextDay();
        InitPageEnabled();
        App.instance.GetMapManager().AllowMouseEvent(true);
        MapController.instance.NextDay();
        GameManager.instance.SetPrioryty(false);
        Debug.Log("4");
    }

    #region Gauage
    public void OnPointerDown(PointerEventData eventData)
    {
        isFilling = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        InitGauageUI();
    }

    void InitGauageUI()
    {
        isFilling = false;
        currentGaugeValue = 0.0f;
        UpdateGaugeUI();
    }

    void FillGauge()
    {
        currentGaugeValue += fillSpeed * Time.deltaTime;
        UpdateGaugeUI();
    }

    void UpdateGaugeUI()
    {
        gaugeImage.fillAmount = currentGaugeValue / maxGaugeValue;
    }
    #endregion

    #region PageSetting
    void InitPageEnabled()
    {
        foreach(NotePage page in pages)
        {
            page.SetPageEnabled(false);
            if (page.GetENotePageType()==ENotePageType.DayStart || page.GetENotePageType() == ENotePageType.SelectEvent)
            {
                page.StopDialogue();
            }
        }
    }

    public NotePage[] GetNotePageArray()
    {
        List<NotePage> todayPages = new List<NotePage>();
        
        foreach (NotePage page in pages)
        {
            if (page.GetPageEnabled())
            {
                todayPages.Add(page);
            }
        }

        return todayPages.ToArray(); ;
    }
    #endregion

    #region QuestSetting
    /// <summary>
    /// 차후 길이 줄일 예정
    /// </summary>
    public void AddMainQuest() 
    { 
        GameObject obj = Instantiate(questPrefab, questParent);
        Quest quest = obj.GetComponent<Quest>();
        quest.SetEQuestType(EQuestType.Main);
        quest.SetText("예시입니다.");
        quest.SetQuestTypeText();
        quest.SetQuestTypeImage();
        SetQuestList();
    }

    public void AddSubQuest()
    {
        GameObject obj = Instantiate(questPrefab, questParent);
        Quest quest = obj.GetComponent<Quest>();
        quest.SetEQuestType(EQuestType.Sub);
        quest.SetText("예시여.");
        quest.SetQuestTypeText();
        quest.SetQuestTypeImage();
        SetQuestList();
    }

    void SetQuestList()
    {
        quests = questParent.GetComponentsInChildren<Quest>();
        foreach (Quest quest in quests)
        {
            if(quest.GetEQuestType() == EQuestType.Sub)
                quest.transform.SetAsLastSibling();
            else
                quest.transform.SetAsFirstSibling();
        }
    }

    void InitQuestList()
    {
        foreach (Quest quest in quests)
        {
            Destroy(quest.gameObject);
            Debug.Log("파괴됨");
        }
    }
    #endregion

}
