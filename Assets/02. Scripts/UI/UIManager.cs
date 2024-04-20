using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : Manager
{
    Dictionary<Type, UIBase> UIDic;
    Stack<UIState> UIStack;

    [SerializeField] Image blackBlur;
    [SerializeField] List<UIBase> UIs;

    public UIState CurrUIState
        => UIStack.Count == 0 ? UIState.Normal : UIStack.Peek();

    [SerializeField] CraftingRawImageController craftingRawImageController;
    [SerializeField] UIHighLightController uiHighLightController;
    [SerializeField] NextDayController nextDayController;
    [SerializeField] PageController pageController;
    [SerializeField] QuestController questController;
    [SerializeField] SoundController soundController;
    [SerializeField] ItemInfoController itemInfoController;
    [SerializeField] UpperController upperController;
    [SerializeField] PopUpController popUpController;
    [SerializeField] InfoController infoController;
    [SerializeField] PVController pvController;

    protected override void Awake()
    {
        base.Awake();

        UIDic = new(UIs.Count);
        UIStack = new();

        foreach (var UI in UIs)
        {
            UIDic.Add(UI.GetPanelType(), UI);
        }

        UIs.Clear(); // clear memory
    }

    void Start()
    {
        InitUIs();
    }

    void InitUIs()
    {
        foreach (var UI in UIDic.Values)
        {
            if (!UI.gameObject.activeSelf) //wake up panels
            {
                UI.gameObject.SetActive(true);
                UI.gameObject.SetActive(false);
            }

            try { UI.Init(); }
            catch (Exception error)
            { Debug.LogError($"ERROR: {error.Message}\n{error.StackTrace}"); }
        }
    }

    void Update() //TODO
    {
        InputKey();
    }

    public void InputKey() //TODO
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isUIStatus(UIState.Menu) == false)
                GetPanel<MenuPanel>().OpenPanel();
            else
                Application.Quit();
        }
    }

    #region Get Panel
    public T GetPanel<T>() where T : UIBase => (T)UIDic[typeof(T)];

    public bool TryGetPanel<T>(out T _panel) where T : UIBase
    {
        if (UIDic.TryGetValue(typeof(T), out var panel))
        {
            _panel = (T)panel;
            return true;
        }

        _panel = default;
        return false;
    }
    #endregion

    #region UI Stack Managing
    public void AddUIStack(UIState _state)
    {
        UIStack.Push(_state);

        Debug.LogError($"PUSH : {_state}, Total stack count: {UIStack.Count}");
    }

    public void PopUIStack()
    {
        UIStack.Pop();
    }

    public bool isUIStatus(UIState _cmp)
    {
        UIStack.TryPeek(out UIState top);
        return _cmp == top;
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
    #endregion

    #region Fade In / Out
    public void FadeIn(Action _endEvent = null)
    {
        if (blackBlur.color.a == 1f)
        {
            _endEvent?.Invoke();
            return;
        }

        blackBlur.gameObject.SetActive(true);

        blackBlur.DOKill();
        blackBlur.DOFade(1f, 0.5f).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _endEvent?.Invoke();
            });
    }

    public void FadeOut(Action _endEvent = null)
    {
        if (blackBlur.color.a == 1f)
        {
            _endEvent?.Invoke();
            return;
        }

        blackBlur.DOFade(0f, 1f).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _endEvent?.Invoke();
                blackBlur.gameObject.SetActive(false);
            });
    }
    #endregion

    public CraftingRawImageController GetCraftingRawImageController()
    {
        return craftingRawImageController;
    }
   
    public UIHighLightController GetUIHighLightController()
    {
        return uiHighLightController;
    }

    public NextDayController GetNextDayController()
    {
        return nextDayController;
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
