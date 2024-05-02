using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


[Serializable]
public class Mode
{
    public ModeCtrl Ctrl;
    public CraftMode Type;
    public Sprite InventorySprite;
}

public class CraftPanel : UIBase
{
    [Header("Controller")]
    [SerializeField] CraftRawCtrl rawCtrl;
    [SerializeField] CraftEffectCtrl effectCtrl;

    [Header("UI")]
    [SerializeField] Image rightImg;
    [SerializeField] Button closeBtn;
    [SerializeField] GameObject hologramBack;
    [SerializeField] Sprite[] btnSprites;

    [Header("Mode")]
    [SerializeField] Mode[] modes;
    [SerializeField] Button[] modeBtns;

    [Header("Fade In / Out UI")]
    [SerializeField] CanvasGroup background;
    [SerializeField] CanvasGroup details;

    public CraftCtrl Craft => (CraftCtrl)modes[0].Ctrl;
    public EquipCtrl Equip => (EquipCtrl)modes[1].Ctrl;
    public BlueprintCtrl Blueprint => (BlueprintCtrl)modes[2].Ctrl;

    public CraftMode ModeType { get; private set; }

    #region Override
    public override void Init()
    {
        foreach (var mode in modes)
            mode.Ctrl.Init();

        SetButtonEvent();

        ModeButtonEvent((int)CraftMode.Craft);

        background.alpha = 0.0f;
        details.alpha = 0.0f;

        gameObject.SetActive(false);
    }

    public override void ReInit() { }

    public override UIState GetUIState() => UIState.Craft;

    public override bool IsAddUIStack() => true;

    public override void OpenPanel()
    {
        base.OpenPanel();

        effectCtrl.StartAnim();
        App.Manager.Sound.PlaySFX("SFX_SceneChange_BaseToCrafting");

        App.Manager.UI.GetPanel<ItemInfoPanel>().HideInfo(); //todo

        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(background.DOFade(1f, 0.5f))
            .Append(details.DOFade(1f, 0.5f));
            //.OnComplete(() => App.Manager.UI.GetItemInfoController().isOpen = true);

        ModeButtonEvent((int)CraftMode.Craft);
    }

    public override void ClosePanel()
    {
        effectCtrl.StopAnim();
        App.Manager.Sound.PlaySFX("SFX_SceneChange_CraftingToBase");
        
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(details.DOFade(0f, 0.5f))
            .Append(background.DOFade(0f, 0.5f))
            .OnComplete(()=>
            {
                base.ClosePanel();

                foreach (var mode in modes)
                    mode.Ctrl.Exit();
            });
    }
    #endregion
    
    void SetButtonEvent()
    {
        for (int i = 0; i < modeBtns.Length; i++)
        {
            int idx = i;

            modeBtns[idx].onClick.AddListener(() =>
            {
                ModeButtonEvent(idx);
            });

            modeBtns[idx].gameObject.SetActive(true);
        }

        closeBtn.onClick.AddListener(() => ClosePanel());
    }

    void ModeButtonEvent(int _idx)
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

        if (ModeType != CraftMode.Blueprint)
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

    public void TurnHologram(bool _isActive)
    {
        hologramBack.SetActive(_isActive);
    }
}
