using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    public UnityAction onSceneLoaded;

    public void LoadScene(int _Idx)
    {
        SceneManager.LoadScene(_Idx);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void LoadSceneAdditive(int _idx)
    {
        SceneManager.LoadScene(_idx, LoadSceneMode.Additive);

        onSceneLoaded?.Invoke();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        onSceneLoaded?.Invoke();
    }
}
