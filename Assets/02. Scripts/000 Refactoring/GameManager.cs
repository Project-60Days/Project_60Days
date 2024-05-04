using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Manager
{
    public ItemSO itemSO;
    [HideInInspector] public List<ItemBase> itemData => itemSO.items.ToList();

    public DayCtrl ctrl;
    [HideInInspector] public bool LoadingComplete = false;
    [HideInInspector] public bool isOver = false;
    [HideInInspector] public bool isHit = false;
    [HideInInspector] public bool isNewDay = true;
    [HideInInspector] public int dayCount = 0;

    [SerializeField] Button nextDayBtn;
    [SerializeField] Button shelterBtn;

    private void Start()
    {
        SetButtonEvent();
        InitItemSO();
        StartCoroutine(InitUI());
    }

    private void SetButtonEvent()
    {
        nextDayBtn.onClick.AddListener(() => NextDay());
        shelterBtn.onClick.AddListener(() => GoToShelter());
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

    private IEnumerator InitUI()
    {
        yield return new WaitUntil(() => App.Manager.Map.mapCtrl.LoadingComplete);

        App.Manager.UI.InitUIs();

        yield return new WaitForSeconds(0.5f);

        LoadingComplete = true;
    }

    public void NextDay()
    {
        ctrl.NextDay();
    }

    public void NewDay()
    {
        App.Manager.Sound.PlayBGM("BGM_InGame");
        App.Manager.UI.FadeOut();
        App.Manager.UI.PopUIStack(UIState.NewDay);

        isHit = false;
        isNewDay = true;
    }

    #region Move Camera
    /// <summary>
    /// To move camera from map to shelter
    /// </summary>
    public void GoToShelter()
    {
        App.Manager.UI.FadeInOut();
        App.Manager.Map.cameraCtrl.GoToShelter();
    }

    /// <summary>
    /// To move camera from shelter to map
    /// </summary>
    public void GoToMap()
    {
        App.Manager.UI.FadeInOut(MoveCameraToMap);
    }

    public void MoveCameraToMap()
    {
        App.Manager.Map.cameraCtrl.GoToMap();
    }
    #endregion
}
