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

    //readonly 

    public partial class Manager
    {
        public static SoundManager Sound => instance.sound;
        public static MapManager Map => instance.map;
        public static UIManager UI => instance.ui;
    }

    public class Data
    {
        //public static DataManager Data => instance.data;
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

    public static bool TryGetDataManager(out DataManager manager)
    {
        manager = Data;
        return manager != null;
    }

    public static bool TryGetSoundManager(out SoundManager manager)
    {
        manager = Sound;
        return manager != null;
    }

    public static bool TryGetMapManager(out MapManager manager)
    {
        manager = Map;
        return manager != null;
    }

    public static bool TryGetUIManager(out UIManager manager)
    {
        manager = UI;
        return manager != null;
    }
}
