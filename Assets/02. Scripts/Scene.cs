using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene : MonoBehaviour
{
    void Start()
    {
        LoadSceneAdditive();
    }

    void LoadSceneAdditive()
    {
        SceneManager.LoadScene("UI Scene", LoadSceneMode.Additive);
        SceneManager.LoadScene("03. Map", LoadSceneMode.Additive);
    }
}
