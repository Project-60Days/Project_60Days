using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{

    public void LoadScene(int _Idx)
    {
        SceneManager.LoadScene(_Idx);
    }

    public void LoadSceneAdditive(int _idx)
    {
        SceneManager.LoadScene(_idx, LoadSceneMode.Additive);
    }
}
