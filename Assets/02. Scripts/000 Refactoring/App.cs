using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DG.Tweening;


public class App : Singleton<App>
{
    readonly SoundManager sound;
    readonly ShelterManager shelter;
    readonly MapManager map;
    readonly UIManager ui;
    readonly TutorialManager tutorial;

    readonly GameData gameData;

    public partial class Manager
    {
        public static SoundManager Sound => instance.sound;
        public static ShelterManager Shelter => instance.shelter;
        public static MapManager Map => instance.map;
        public static UIManager UI => instance.ui;
        public static TutorialManager Tutorial => instance.tutorial;
    }

    public class Data
    {
        public static GameData Game => instance.gameData;
    }

    #region Load Scene
    public static void LoadScene(SceneName _type)
    {
        DOTween.KillAll();
        SceneManager.LoadScene((int)_type);
    }

    public static void LoadSceneAdditive(SceneName _type)
    {
        SceneManager.LoadScene((int)_type, LoadSceneMode.Additive);
    }
    #endregion

    #region Try Get Manager
    public static bool TryGetSoundManager(out SoundManager manager)
    {
        manager = Manager.Sound;
        return manager != null;
    }

    public static bool TryGetShelterManager(out ShelterManager manager)
    {
        manager = Manager.Shelter;
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

    public static bool TryGetTutorialManager(out TutorialManager manager)
    {
        manager = Manager.Tutorial;
        return manager != null;
    }
    #endregion
}
