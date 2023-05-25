using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene : MonoBehaviour
{
    void Start()
    {
        LoadUISceneAdditive();
        LoadMapSceneAdditive();
    }

    void LoadUISceneAdditive()
    {
        SceneManager.LoadScene("UI Scene", LoadSceneMode.Additive);
    }

    void LoadMapSceneAdditive()
    {
        SceneManager.LoadScene("03. Map", LoadSceneMode.Additive);
    }
}
