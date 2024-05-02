using UnityEngine;
using DG.Tweening;

public class Singleton<T> where T : Singleton<T>, new()
{
    protected static T _instance = null;

    /// <summary>
    /// Checks whether a singleton has been created and returns if so.
    /// If not created, create and return.
    /// </summary>
    public static T instance
    {
        get
        {
            _instance ??= new();
            return _instance;
        }
    }
}

public class AppHelper : MonoBehaviour
{
    private static GameObject _helperObject = null;

    private void Awake()
    {
        if (_helperObject != null)
        {
            DestroyImmediate(_helperObject);
        }

        _helperObject = gameObject;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 1; // force vsync
        Application.targetFrameRate = 120;

        DOTween.safeModeLogBehaviour = DG.Tweening.Core.Enums.SafeModeLogBehaviour.Error;

        Screen.SetResolution(
            1920,
            1080,
            false);
    }

    private void OnApplicationQuit()
    {
        DestroyImmediate(_helperObject);
    }
}