using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BenchPanel : UIBase
{
    [Header("Controller")]
    [SerializeField] BenchRawCtrl rawCtrl;

    [Header("UI")]
    [SerializeField] Button closeBtn;
    [SerializeField] Sprite[] btnSprites;

    [Header("Mode")]
    [SerializeField] ModeCtrl[] modes;
    [SerializeField] Button[] modeBtns;

    [Header("Fade In / Out UI")]
    [SerializeField] CanvasGroup background;
    [SerializeField] CanvasGroup details;

    public CraftCtrl Craft => modes[0] as CraftCtrl;
    public EquipCtrl Equip => modes[1] as EquipCtrl;
    public BlueprintCtrl Blueprint => modes[2] as BlueprintCtrl;

    public BenchType BenchMode { get; private set; }

    #region Override
    public override void Init()
    {
        foreach (var mode in modes)
        {
            mode.Init();
        }

        SetButtonEvent();

        gameObject.SetActive(false);
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
                    DeactiveMode(mode);
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
        }

        closeBtn.onClick.AddListener(ClosePanel);
    }

    private void ModeButtonEvent(int _idx)
    {
        for (int i = 0; i < modeBtns.Length; i++)
        {
            if (i == _idx)
            {
                modeBtns[i].image.sprite = btnSprites[1];
                ActiveMode(modes[i]);
            }
            else
            {
                modeBtns[i].image.sprite = btnSprites[0];
                DeactiveMode(modes[i]);
            }
        }
    }

    private void ActiveMode(ModeCtrl _mode)
    {
        _mode.Enter();

        BenchMode = _mode.GetModeType();
        rawCtrl.InitTarget();
    }

    private void DeactiveMode(ModeCtrl _mode)
    {
        _mode.Exit();
    }
}
