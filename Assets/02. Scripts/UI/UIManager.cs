using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : Manager, IListener
{
    [SerializeField] Image blackBlur;
    [SerializeField] List<UIBase> UIs;

    [HideInInspector] public UIState CurrState
        => UIStack.Count == 0 ? UIState.Normal : UIStack.Peek();
    
    private Dictionary<Type, UIBase> UIDic;
    private Stack<UIState> UIStack;

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

        App.Manager.Event.AddListener(EventCode.NextDayStart, this);
        App.Manager.Event.AddListener(EventCode.NextDayMiddle, this);
        App.Manager.Event.AddListener(EventCode.NextDayEnd, this);
    }

    public void OnEvent(EventCode _code, Component _sender, object _param = null)
    {
        switch (_code)
        {
            case EventCode.NextDayStart:
                //AddUIStack(UIState.NewDay);
                break;

            case EventCode.NextDayMiddle:
                AddUIStack(UIState.NewDay);
                ReInitUIs();
                break;

            case EventCode.NextDayEnd:
                FadeOut();
                PopUIStack(UIState.NewDay);
                break;
        }
    }

    private void Start()
    {
        InitUIs();
    }

    private void InitUIs()
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

    public void ReInitUIs()
    {
        foreach (var UI in UIDic.Values)
        {
            if (!UI.gameObject.activeSelf) //wake up panels
            {
                UI.gameObject.SetActive(true);
                UI.gameObject.SetActive(false);
            }

            try { UI.ReInit(); }
            catch (Exception error)
            { Debug.LogError($"ERROR: {error.Message}\n{error.StackTrace}"); }
        }
    }

    private void Update() //TODO
    {
        InputKey();
    }

    private void InputKey() //TODO
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CurrState == UIState.Normal)
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
    }

    public void PopUIStack(UIState _state = 0)
    {
        if (CurrState != _state) return;

        UIStack.Pop();
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
        App.Manager.Sound.StopBGM();

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
        if (blackBlur.color.a == 0f)
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

    public void FadeInOut(Action _midEvent = null)
    {
        App.Manager.Sound.StopBGM();

        if (blackBlur.color.a == 1f)
        {
            _midEvent?.Invoke();
            FadeOut();
            return;
        }

        blackBlur.gameObject.SetActive(true);

        blackBlur.DOKill();
        blackBlur.DOFade(1f, 0.5f).SetEase(Ease.Linear)
           .OnComplete(() =>
           {
               _midEvent?.Invoke();
               FadeOut();
           });
    }
    #endregion
}
