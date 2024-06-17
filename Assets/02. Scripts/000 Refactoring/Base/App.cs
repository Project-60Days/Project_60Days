using UnityEngine.SceneManagement;
using DG.Tweening;


public class App : Singleton<App>
{
    #region Variables
    private readonly SoundManager sound;
    private readonly GameManager game;
    private readonly ShelterManager shelter;
    private readonly MapManager map;
    private readonly UIManager ui;
    private readonly TutorialManager tutorial;
    private readonly AssetManager asset;

    private readonly GameData gameData;
    private readonly SettingData settingData;
    private readonly TestData testData;
    #endregion

    public partial class Manager
    {
        public static SoundManager Sound => instance.sound;
        public static GameManager Game => instance.game;
        public static ShelterManager Shelter => instance.shelter;
        public static MapManager Map => instance.map;
        public static UIManager UI => instance.ui;
        public static TutorialManager Tutorial => instance.tutorial;
        public static AssetManager Asset => instance.asset;
    }

    public class Data
    {
        public static GameData Game => instance.gameData;
        public static SettingData Setting => instance.settingData;
        public static TestData Test => instance.testData;
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

    public static bool TryGetGameManager(out GameManager manager)
    {
        manager = Manager.Game;
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

    public static bool TryGetAssetManager(out AssetManager manager)
    {
        manager = Manager.Asset;
        return manager != null;
    }
    #endregion
}
