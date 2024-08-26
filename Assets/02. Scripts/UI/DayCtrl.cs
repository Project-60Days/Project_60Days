using System.Collections;
using UnityEngine;
using TMPro;

public class DayCtrl : MonoBehaviour, IListener
{
    private enum NewDayState
    {
        Normal,
        Hit,
        Die
    }

    [SerializeField] Transform blackPanel;
    [SerializeField] GameObject dayCountPrefab;

    private GameManager Game;

    private GameObject dayCount;
    private NewDayState todayState;

    private void Awake()
    {
        App.Manager.Event.AddListener(EventCode.NextDayMiddle, this);
    }

    public void OnEvent(EventCode _code, Component _sender, object _param = null)
    {
        switch (_code)
        {
            case EventCode.NextDayMiddle:
                StartCoroutine(SetNextDay());
                break;
        }
    }

    private void Start()
    {
        Game = App.Manager.Game;
    }

    private IEnumerator SetNextDay()
    {
        SetState();
        CreateDayCountTxt(GetText());

        yield return new WaitForSeconds(2f);

        StartNewDay();
    }

    private void SetState()
    {
        if (Game.isOver)
            todayState = NewDayState.Die;
        else if (Game.isHit)
            todayState = NewDayState.Hit;
        else
            todayState = NewDayState.Normal;
    }

    private string GetText() => todayState switch
    {
        NewDayState.Normal => "<color=white>Day " + "{vertexp}" + Game.dayCount.ToString() + "{/vertexp}</color>",
        NewDayState.Hit => "<color=red><shake a=0.1>" + "Day " + "{vertexp}" + Game.dayCount.ToString() + "{/vertexp}</shake></color>",
        NewDayState.Die => "<color=red><shake a=0.1>GAME OVER</shake></color>",
        _ => null,
    };

    private void CreateDayCountTxt(string _text)
    {
        dayCount = Instantiate(dayCountPrefab, blackPanel);
        TextMeshProUGUI text = dayCount.GetComponent<TextMeshProUGUI>();
        text.text = _text;
    }

    private void StartNewDay()
    {
        switch (todayState)
        {
            case NewDayState.Die:
                Application.Quit();
                break;

            case NewDayState.Hit:
                App.Manager.Event.PostEvent(EventCode.Hit, this);
                goto case NewDayState.Normal;

            case NewDayState.Normal:
                Destroy(dayCount);
                App.Manager.Event.PostEvent(EventCode.NextDayEnd, this);
                break;
        }
    }
}