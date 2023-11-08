using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EControllerType
{
    NONE, MAP, CRAFT, INVENTORY, NOTE, SELECT, NEXTDAY
}

public enum EManagerType
{
    NONE, DATA, SOUND, MAP, QUEST
}

public class App : Singleton<App>
{
    public Dictionary<EControllerType, ControllerBase> dic_controllers;
    public Dictionary<EManagerType, ManagementBase> dic_managers;

    private void Awake()
    {
        dic_controllers = new Dictionary<EControllerType, ControllerBase>();
        dic_managers = new Dictionary<EManagerType, ManagementBase>();

        DontDestroyOnLoad(this.gameObject);

        var controller = GetComponentsInChildren<ControllerBase>(true);
        var manager = GetComponentsInChildren<ManagementBase>(true);

        foreach (var c in controller)
        {
            dic_controllers.Add(c.GetControllerType(), c);
        }

        foreach (var m in manager)
        {
            dic_managers.Add(m.GetManagemetType(), m);
        }
    }

    public bool HasController(EControllerType _type)
    {
        return dic_controllers.ContainsKey(_type);
    }

    public MapUiController GetMapUiController()
    {
        if (!HasController(EControllerType.MAP))
            return null;

        return dic_controllers[EControllerType.MAP] as MapUiController;
    }

    public CraftingUiController GetCraftController()
    {
        if (!HasController(EControllerType.CRAFT))
            return null;

        return dic_controllers[EControllerType.CRAFT] as CraftingUiController;
    }

    public InventoryController GetInventoryController()
    {
        if (!HasController(EControllerType.INVENTORY))
            return null;

        return dic_controllers[EControllerType.INVENTORY] as InventoryController;
    }

    public NoteController GetNoteController()
    {
        if (!HasController(EControllerType.NOTE))
            return null;

        return dic_controllers[EControllerType.NOTE] as NoteController;
    }

    public SelectController GetSelectController()
    {
        if (!HasController(EControllerType.SELECT))
            return null;

        return dic_controllers[EControllerType.SELECT] as SelectController;
    }

    public NextDayController GetNextDayController()
    {
        if (!HasController(EControllerType.NEXTDAY))
            return null;

        return dic_controllers[EControllerType.NEXTDAY] as NextDayController;
    }

    public bool HasManager(EManagerType _type)
    {
        return dic_managers.ContainsKey(_type);
    }

    public DataManager GetDataManager()
    {
        if (!HasManager(EManagerType.DATA))
            return null;

        return dic_managers[EManagerType.DATA] as DataManager;
    }

    public SoundManager GetSoundManager()
    {
        if (!HasManager(EManagerType.SOUND))
            return null;

        return dic_managers[EManagerType.SOUND] as SoundManager;
    }

    public MapManager GetMapManager()
    {
        if (!HasManager(EManagerType.MAP))
            return null;

        return dic_managers[EManagerType.MAP] as MapManager;
    }

    public QuestManager GetQuestManager()
    {
        if (!HasManager(EManagerType.QUEST))
            return null;

        return dic_managers[EManagerType.QUEST] as QuestManager;
    }

    public void AddController(ControllerBase controller)
    {
        dic_controllers.Add(controller.GetControllerType(), controller);
    }
}
