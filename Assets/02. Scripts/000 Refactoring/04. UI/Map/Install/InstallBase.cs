using UnityEngine;
using UnityEngine.UI;

public abstract class InstallBase : IconBase, IListener
{
    protected abstract string GetItemCode();
    protected abstract DroneType GetDroneType();

    private InventoryPanel inventory;
    private DroneUnit drone;

    private void Awake()
    {
        App.Manager.Event.AddListener(EventCode.ItemUpdate, this);
    }

    public void OnEvent(EventCode _code, Component _sender, object _param = null)
    {
        switch (_code)
        {
            case EventCode.ItemUpdate:
                var isExist = inventory.CheckItemExist(GetItemCode());
                gameObject.SetActive(isExist);
                break;
        }
    }

    protected override void Start()
    {
        base.Start();

        inventory = App.Manager.UI.GetPanel<InventoryPanel>();
        drone = App.Manager.Map.GetUnit<DroneUnit>();

        GetComponent<Button>().onClick.AddListener(OnClickEvent);
    }

    protected virtual void OnClickEvent()
    {
        if (App.Manager.Map.CanClick)
        {
            drone.Prepare(DroneType.Disruptor);
        }
    }
}
