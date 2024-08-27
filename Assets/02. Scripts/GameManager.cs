using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Manager, IListener
{
    public ItemSO itemSO;
    [HideInInspector] public List<ItemBase> itemData => itemSO.items.ToList();

    public DayCtrl ctrl;
    [HideInInspector] public bool isOver = false;
    [HideInInspector] public bool isHit = false;
    [HideInInspector] public bool isNewDay = true;
    [HideInInspector] public int dayCount = 0;
    [HideInInspector] public bool startTutorial = false;
    [HideInInspector] public int durability;



    protected override void Awake()
    {
        base.Awake();

        startTutorial = App.Data.Test.startTutorial;

        App.Manager.Event.AddListener(EventCode.GameStart, this);
        App.Manager.Event.AddListener(EventCode.NextDayEnd, this);
    }

    public void OnEvent(EventCode _code, Component _sender, object _param = null)
    {
        switch (_code)
        {
            case EventCode.GameStart:
                if (App.Manager.Game.startTutorial)
                {
                    App.Manager.Event.PostEvent(EventCode.TutorialStart, this);
                }
                else
                {
                    App.Manager.Event.PostEvent(EventCode.TutorialEnd, this);
                }
                break;

            case EventCode.NextDayEnd:
                NewDay();
                break;
        }
    }

    private void Start()
    {
        
        InitItemSO();

        durability = App.Data.Test.Map.durability;
    }

   

    private void InitItemSO()
    {
        var itemData = App.Data.Game.itemData;

        foreach (var item in itemSO.items)
        {
            item.data = itemData[item.Code];
            item.Init();
        }
    }

    public void NewDay()
    {
        App.Manager.Sound.PlayBGM("BGM_InGame");

        isHit = false;
        isNewDay = true;
    }

   

    public void CompleteQuest(string _currCode, string _nextCode = null)
    {
        StartCoroutine(WaitForNormal(_currCode, _nextCode));
    }

    private IEnumerator WaitForNormal(string _currCode, string _nextCode = null)
    {
        yield return new WaitUntil(() => App.Manager.UI.CurrState == UIState.Normal);
        App.Manager.UI.GetPanel<QuestPanel>().EndQuest(_currCode, _nextCode);
    }

    public void ChangeDurbility(int amount)
    {
        if (durability + amount > 0)
            durability += amount;

        //UIManager.instance.GetUpperController().UpdateDurabillity(); 
    }

    public void TakeDamage(int zombieCount)
    {
        if (isOver)
            return;

        // 피격 애니메이션
        if (durability - zombieCount > 0)
        {
            durability -= zombieCount;
            App.Manager.Game.isHit = true;
        }
        else if (durability - zombieCount <= 0)
        {
            // 내구도가 0이 되면 게임 오버
            durability = 0;
            isOver = true;
            App.Manager.Game.isHit = true;
            Debug.Log("내구도 부족. 게임 오버");

            // 게임 오버
            App.Manager.Game.isOver = true;
        }
    }
}
