using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using DG.Tweening;
using TMPro;

public class DayCtrl : MonoBehaviour
{
    private GameManager game;

    [SerializeField] Image blackPanel;
    [SerializeField] GameObject dayCountPrefab;
    GameObject dayCount;
    [SerializeField] MapIcon mapIcon;

    private void Start()
    {
        game = App.Manager.Game;
    }

    public void InitBlackPanel()
    {
        if (game.isHit == true)
        {
            App.Manager.Shelter.cameraCtrl.Shake();
        }

        App.Manager.UI.FadeOut();

        Destroy(dayCount);

        game.isHit = false;

        App.Manager.UI.PopUIStack();
        //todo

        App.Manager.UI.GetPanel<NotePanel>().isNewDay = true;
    }

    public void EndFadeIn()
    {
        StartCoroutine(NextDayEventCallBack(() =>
        {
            if (game.isOver == true)
                StartCoroutine(ShowGameOver());
            else
                StartCoroutine(ShowNextDate());
        }));
    }

    /// <summary>
    /// NextDayEvent 콜백함수
    /// </summary>
    IEnumerator NextDayEventCallBack(System.Action callback)
    {
        App.Manager.Map.cameraCtrl.ReInit();

        yield return StartCoroutine(App.Manager.Map.NextDayCoroutine());

        yield return new WaitForSeconds(1f);

        App.Manager.UI.GetPanel<PagePanel>().ReInit();

        callback?.Invoke();
    }

    IEnumerator ShowNextDate()
    {
        App.Manager.UI.GetPanel<NotePanel>().ReInit();
        App.Manager.UI.GetPanel<CraftPanel>().Equip.EquipItemDayEvent();

        int today = App.Manager.UI.GetPanel<NotePanel>().dayCount;

        string text = "<color=white>Day " + "{vertexp}" + today.ToString() + "{/vertexp}</color>";

        if (game.isHit == true)
            text = "<color=red><shake a=0.1>" + "Day " + "{vertexp}" + today.ToString() + "{/vertexp}</shake></color>";

        CreateDayCountTxt(text);
        mapIcon.SetIconImage();

        yield return new WaitForSeconds(2f);

        InitBlackPanel();

        App.Manager.Sound.PlayBGM("BGM_InGameTheme");
    }


    IEnumerator ShowGameOver()
    {
        string text = "<color=red><shake a=0.1>GAME OVER</shake></color>";
        CreateDayCountTxt(text);

        yield return new WaitForSeconds(2f);

        Application.Quit();
    }

    void CreateDayCountTxt(string _text)
    {
        dayCount = Instantiate(dayCountPrefab, blackPanel.transform);
        TextMeshProUGUI text = dayCount.GetComponent<TextMeshProUGUI>();
        text.text = _text;
    }
}