using UnityEngine;

public abstract class OptionBase : MonoBehaviour
{
    protected SettingData Setting;

    protected virtual void Awake()
    {
        Setting = App.Data.Setting;
    }

    protected virtual void Start()
    {
        SetString();
        SetEvent();
    }

    protected virtual void OnEnable()
    {
        SetValueFromData();
    }

    protected abstract void SetString();

    protected abstract void SetEvent();

    protected abstract void SetValueFromData();

    public abstract void SaveOption();
}
