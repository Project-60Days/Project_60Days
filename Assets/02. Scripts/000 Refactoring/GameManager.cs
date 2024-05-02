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

    protected override void Awake()
    {
        base.Awake();

        SetButtonEvent();
        InitItemSO();
    }

    private void SetButtonEvent()
    {
        nextDayBtn.onClick.AddListener(() => NextDay());
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
        //StartCoroutine(NextDayEventCallBack(() =>
        //{
        //    if (isOver == true)
        //        StartCoroutine(ShowGameOver());
        //    else
        //        StartCoroutine(ShowNextDate());
        //}));
    }
}
