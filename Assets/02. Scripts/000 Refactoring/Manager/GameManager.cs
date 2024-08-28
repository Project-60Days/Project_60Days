using UnityEngine;

public enum NewDayState
{
    Normal,
    Hit,
    Die
}

public class GameManager : Manager, IListener
{
    [HideInInspector] public int DayCount { get; private set; } = 0;

    private int durability;

    public int Durability
    {
        get { return durability; }
        set
        {
            if (value < durability)
            {
                TodayState = NewDayState.Hit;

                if (value <= 0)
                {
                    TodayState = NewDayState.Die;
                    durability = 0; 
                }
                else
                {
                    durability = value;
                }
            }
            else
            {
                TodayState = NewDayState.Normal;
                durability = value;
            }
        }
    }

    public NewDayState TodayState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        App.Manager.Event.AddListener(EventCode.GameStart, this);
        App.Manager.Event.AddListener(EventCode.NextDayStart, this);
        App.Manager.Event.AddListener(EventCode.NextDayEnd, this);
    }

    public void OnEvent(EventCode _code, Component _sender, object _param = null)
    {
        switch (_code)
        {
            case EventCode.GameStart:
                if (App.Data.Test.startTutorial)
                {
                    DayCount = -1;
                    App.Manager.Event.PostEvent(EventCode.TutorialStart, this);
                }
                else
                {
                    App.Manager.Event.PostEvent(EventCode.TutorialEnd, this);
                }
                break;

            case EventCode.NextDayStart:
                DayCount++;
                break;

            case EventCode.NextDayEnd:
                NewDay();
                break;
        }
    }

    private void Start()
    {
        durability = App.Data.Test.Map.durability;
    }

    private void NewDay()
    {
        App.Manager.Sound.PlayBGM("BGM_InGame");

        switch (TodayState)
        {
            case NewDayState.Die:
                Application.Quit();
                break;

            case NewDayState.Hit:
                App.Manager.Event.PostEvent(EventCode.Hit, this);
                break;
        }

        TodayState = NewDayState.Normal;
    }
}
