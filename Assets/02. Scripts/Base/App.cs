using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DG.Tweening;


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

    readonly DataManager data;
    readonly SoundManager sound;
    readonly MapManager map;
    readonly UIManager ui;

    public static DataManager Data => instance.data;
    public static SoundManager Sound => instance.sound;
    public static MapManager Map => instance.map;
    public static UIManager UI => instance.ui;

    public static void LoadScene(ESceneType _type)
    {
        DOTween.KillAll();
        SceneManager.LoadScene((int)_type);
    }

    public static void LoadSceneAdditive(ESceneType _type)
    {
        SceneManager.LoadScene((int)_type, LoadSceneMode.Additive);
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
