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
    private readonly EventManager eventM;

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
        public static EventManager Event => instance.eventM;
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
}
