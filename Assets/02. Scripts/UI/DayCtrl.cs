using System.Collections;
using UnityEngine;
using TMPro;

public class DayCtrl : MonoBehaviour
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
    private UIManager UI;
    private MapManager Map;

    private GameObject dayCount;
    private NewDayState todayState;

    private void Start()
    {
        Game = App.Manager.Game;
        Map = App.Manager.Map;
        UI = App.Manager.UI;
    }

    public void NextDay()
    {
        UI.FadeIn(FadeCallBack);
    }

    private void FadeCallBack()
    {
        Map.cameraCtrl.ResetCamera();

        UI.AddUIStack(UIState.NewDay);

        StartCoroutine(SetNextDay());
    }

    private IEnumerator SetNextDay()
    {
        App.Manager.Test.NextDay();
        Map.NextDay();

        yield return new WaitForSeconds(1f);

        UI.ReInitUIs();

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
                App.Manager.Shelter.cameraCtrl.Shake();
                goto case NewDayState.Normal;

            case NewDayState.Normal:
                Destroy(dayCount);
                Game.NewDay();
                break;
        }
    }
}