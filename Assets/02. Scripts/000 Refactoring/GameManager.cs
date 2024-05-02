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
    [HideInInspector] public bool isOver = false;
    [HideInInspector] public bool isHit = false;
    [HideInInspector] public bool isNewDay = true;

    [SerializeField] Button nextDayBtn;
    [SerializeField] Button shelterBtn;

    protected override void Awake()
    {
        base.Awake();

        SetButtonEvent();
        InitItemSO();
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

    public void NextDay()
    {
        App.Manager.UI.AddUIStack(UIState.EndDay);

        App.Manager.UI.FadeIn(EndFadeIn);
    }

    private void EndFadeIn()
    {
        ctrl.EndFadeIn();
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
