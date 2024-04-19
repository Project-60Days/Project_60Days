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
    readonly SoundManager sound;
    readonly MapManager map;
    readonly UIManager ui;

    readonly GameDatasss gameData;

    public partial class Manager
    {
        public static SoundManager Sound => instance.sound;
        public static MapManager Map => instance.map;
        public static UIManager UI => instance.ui;
    }

    public class Data
    {
        public static GameDatasss Game => instance.gameData;
    }
   
    

    public static void LoadScene(ESceneType _type)
    {
        DOTween.KillAll();
        SceneManager.LoadScene((int)_type);
    }

    public static void LoadSceneAdditive(ESceneType _type)
    {
        SceneManager.LoadScene((int)_type, LoadSceneMode.Additive);
    }

    public static bool TryGetSoundManager(out SoundManager manager)
    {
        manager = Manager.Sound;
        return manager != null;
    }

    public static bool TryGetMapManager(out MapManager manager)
    {
        manager = Manager.Map;
        return manager != null;
    }

    public static bool TryGetUIManager(out UIManager manager)
    {
        manager = Manager.UI;
        return manager != null;
    }
}
