using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Manager
{
    [SerializeField] NoteController noteController;
    [SerializeField] InventoryController inventoryController;
    [SerializeField] CraftingUiController craftingUiController;
    [SerializeField] CraftingRawImageController craftingRawImageController;
    [SerializeField] CraftModeController craftModeController;
    [SerializeField] UIHighLightController uiHighLightController;
    [SerializeField] SelectController selectController;
    [SerializeField] NextDayController nextDayController;
    [SerializeField] AlertController alertController;
    [SerializeField] MenuController menuController;
    [SerializeField] PageController pageController;
    [SerializeField] QuestController questController;
    [SerializeField] SoundController soundController;
    [SerializeField] ItemInfoController itemInfoController;
    [SerializeField] UpperController upperController;
    [SerializeField] PopUpController popUpController;
    [SerializeField] InfoController infoController;
    [SerializeField] PVController pvController;


    public Stack<UIState> currUIStack = new Stack<UIState>();

    protected override void Awake()
    {
        base.Awake();

        currUIStack.Push(UIState.Normal);
    }
    void Update()
    {
        InputKey();
    }

    public void InputKey()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isUIStatus(UIState.Menu) == false)
                menuController.EnterMenu();
            else
                menuController.QuitMenu();
        }
    }

    public void AddCurrUIName(UIState _state)
    {
        currUIStack.Push(_state);

        Debug.LogError("currUIStack : " + currUIStack.Peek());
    }

    public void PopCurrUI()
    {
        currUIStack.Pop();
    }

    public UIState StringToState(string _state) => _state switch
    {
        "UI_NORMAL" => UIState.Normal,
        "UI_MAP" => UIState.Map,
        "UI_NOTE" => UIState.Note,
        "UI_CRAFTING" => UIState.Craft,
        "UI_SELECT" => UIState.Select,
        "UI_PV" => UIState.PV,
        "UI_POPUP" => UIState.PopUp,
        "UI_LOADING" => UIState.Loading,
        "UI_MENU" => UIState.Menu,
        _ => UIState.Normal,

    };

    public bool isUIStatus(UIState _cmp)
    {
        currUIStack.TryPeek(out UIState top);
        return _cmp == top;
    }

    public NoteController GetNoteController()
    {
        return noteController;
    }

    public InventoryController GetInventoryController()
    {
        return inventoryController;
    }

    public CraftingUiController GetCraftingUiController()
    {
        return craftingUiController;
    }

    public CraftingRawImageController GetCraftingRawImageController()
    {
        return craftingRawImageController;
    }

    public CraftModeController GetCraftModeController()
    {
        return craftModeController;
    }
   
    public UIHighLightController GetUIHighLightController()
    {
        return uiHighLightController;
    }

    public SelectController GetSelectController()
    {
        return selectController;
    }

    public NextDayController GetNextDayController()
    {
        return nextDayController;
    }

    public AlertController GetAlertController()
    {
        return alertController;
    }

    public MenuController GetMenuController()
    {
        return menuController;
    }

    public PageController GetPageController()
    {
        return pageController;
    }

    public QuestController GetQuestController()
    {
        return questController;
    }

    public SoundController GetSoundController()
    {
        return soundController;
    }

    public ItemInfoController GetItemInfoController()
    {
        return itemInfoController;
    }

    public UpperController GetUpperController()
    {
        return upperController;
    }

    public PopUpController GetPopUpController()
    {
        return popUpController;
    }

    public InfoController GetInfoController()
    {
        return infoController;
    }

    public PVController GetPVController()
    {
        return pvController;
    }
}
