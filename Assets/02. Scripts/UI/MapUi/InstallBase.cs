using UnityEngine;
using UnityEngine.UI;

public abstract class InstallBase : IconBase, IListener
{
    protected abstract string GetItemCode();

    private void Awake()
    {
        App.Manager.Event.AddListener(EventCode.ItemUpdate, this);
    }

    public void OnEvent(EventCode _code, Component _sender, object _param = null)
    {
        switch (_code)
        {
            case EventCode.ItemUpdate:
                var isExist = App.Manager.UI.GetPanel<InventoryPanel>().CheckItemExist(GetItemCode());
                gameObject.SetActive(isExist);
                break;
        }
    }

    protected override void Start()
    {
        base.Start();

        GetComponent<Button>().onClick.AddListener(OnClickEvent);
    }

    protected abstract void OnClickEvent();
}
