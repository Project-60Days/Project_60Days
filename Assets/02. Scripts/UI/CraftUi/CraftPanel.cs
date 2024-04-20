using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


[Serializable]
public class Mode
{
    public ModeCtrl Ctrl;
    public CraftMode Type;
    public Sprite InventorySprite;
}

public class CraftPanel : UIBase
{
    [SerializeField] Image inventoryImage;
    [SerializeField] Button closeBtn;

    [SerializeField] Mode[] modes;
    [SerializeField] Button[] modeBtns;
    [SerializeField] Sprite[] btnSprites;

    [SerializeField] GameObject hologramBack;

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

        gameObject.SetActive(false);
    }

    public override void ReInit() 
    {
        App.Manager.UI.GetItemInfoController().HideInfo(); //todo

        ModeButtonEvent((int)CraftMode.Craft);
    }

    public override UIState GetUIState() => UIState.Craft;

    public override bool IsAddUIStack() => true;

    public override void OpenPanel()
    {
        base.OpenPanel();

        ModeButtonEvent((int)CraftMode.Craft);
    }

    public override void ClosePanel()
    {
        App.Manager.UI.GetPanel<InventoryPanel>().ClosePanel();

        base.ClosePanel();

        foreach (var mode in modes)
            mode.Ctrl.Exit();
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

        inventoryImage.sprite = _mode.InventorySprite;

        App.Manager.UI.GetCraftingRawImageController().DestroyObject();
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
