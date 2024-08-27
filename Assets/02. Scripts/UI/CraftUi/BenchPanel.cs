using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


[Serializable]
public class Mode
{
    public ModeCtrl Ctrl;
    public BenchMode Type;
    public Sprite InventorySprite;
}

public class BenchPanel : UIBase
{
    [Header("Controller")]
    [SerializeField] CraftRawCtrl rawCtrl;

    [Header("UI")]
    [SerializeField] Image rightImg;
    [SerializeField] Button closeBtn;
    [SerializeField] Sprite[] btnSprites;

    [Header("Mode")]
    [SerializeField] Mode[] modes;
    [SerializeField] Button[] modeBtns;

    [Header("Fade In / Out UI")]
    [SerializeField] CanvasGroup background;
    [SerializeField] CanvasGroup details;

    public CraftCtrl Craft => modes[0].Ctrl as CraftCtrl;
    public EquipCtrl Equip => modes[1].Ctrl as EquipCtrl;
    public BlueprintCtrl Blueprint => modes[2].Ctrl as BlueprintCtrl;

    public BenchMode ModeType { get; private set; }

    #region Override
    public override void Init()
    {
        foreach (var mode in modes)
        {
            mode.Ctrl.Init();
        }

        SetButtonEvent();

        gameObject.SetActive(false);
    }

    public override void ReInit() 
    {
        Equip.EquipItemDayEvent();
    }

    public override UIState GetUIState() => UIState.Bench;

    public override bool IsAddUIStack() => true;

    public override void OpenPanel()
    {
        base.OpenPanel();

        App.Manager.Sound.PlaySFX("SFX_Craft_Open");

        App.Manager.UI.GetPanel<ItemInfoPanel>().HideInfo(); //todo

        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(background.DOFade(1f, 0.5f))
            .Append(details.DOFade(1f, 0.5f));
            //.OnComplete(() => App.Manager.UI.GetItemInfoController().isOpen = true);

        ModeButtonEvent(0);
    }

    public override void ClosePanel()
    {
        App.Manager.Sound.PlaySFX("SFX_Craft_Close");
        
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(details.DOFade(0f, 0.5f))
            .Append(background.DOFade(0f, 0.5f))
            .OnComplete(()=>
            {
                base.ClosePanel();

                foreach (var mode in modes)
                {
                    mode.Ctrl.Exit();
                }
            });
    }
    #endregion
    
    private void SetButtonEvent()
    {
        for (int i = 0; i < modeBtns.Length; i++)
        {
            int idx = i;

            modeBtns[idx].onClick.AddListener(() => ModeButtonEvent(idx));
            modeBtns[idx].gameObject.SetActive(true);
        }

        closeBtn.onClick.AddListener(ClosePanel);
    }

    private void ModeButtonEvent(int _idx)
    {
        for (int i = 0; i < modeBtns.Length; i++)
        {
            if (_idx == i)
            {
                modeBtns[i].image.sprite = btnSprites[1];
                ActiveMode(modes[i]);
            }
            else
            {
                modeBtns[i].image.sprite = btnSprites[0];
                InActiveMode(modes[i]);
            }
        }
    }

    void ActiveMode(Mode _mode)
    {
        ModeType = _mode.Type;

        _mode.Ctrl.gameObject.SetActive(true);

        if (ModeType != BenchMode.Blueprint)
            App.Manager.UI.GetPanel<InventoryPanel>().OpenPanel();
        else
            App.Manager.UI.GetPanel<InventoryPanel>().ClosePanel();

        rightImg.sprite = _mode.InventorySprite;

        rawCtrl.InitTarget();
    }

    void InActiveMode(Mode _mode)
    {
        _mode.Ctrl.gameObject.SetActive(false);
        _mode.Ctrl.Exit();
    }
}
