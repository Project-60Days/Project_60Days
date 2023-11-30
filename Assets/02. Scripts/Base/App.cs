using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EControllerType
{
    NONE, MAP
}

public enum EManagerType
{
    NONE, DATA, SOUND, MAP
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

    public void AddController(ControllerBase controller)
    {
        dic_controllers.Add(controller.GetControllerType(), controller);
    }
}
