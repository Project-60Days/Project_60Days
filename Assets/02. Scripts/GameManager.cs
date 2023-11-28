using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct Controller
{
    bool isGameOver;
}

/// <summary>
/// �� ���� �Ѿ�� ���� �°� ��, ĳ����, �½�ũ ������Ʈ. -> �� �Ŵ������� ����.
/// UI, ��ȭ �ý��� ��Ʈ��.
/// ���� ���� �� ����, ���� ����, ���� ����.
/// �� ����.
/// </summary>
public class GameManager : Singleton<GameManager>
{
    Controller controller;
    MapCamera mapCamera;
    
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        InputKey();
    }

    public void InputKey()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
            //QuitGame();
    }

    public void UpdateAllState()
    {
        // 1. ���� ����

        // 2. �� ������Ʈ

        // 3. ĳ���� ���� ������Ʈ

        // 4. �׽�ũ ������Ʈ
    }

    public void SaveGame()
    {
        // ���� ��� ������ ����
    }

    public void PrevGameStart()
    {
        // ���� ������ �ҷ�����

        // �� �̵�
        SceneLoader.instance.LoadScene(1);
    }

    public void NewGameStart()
    {
        // ������ �ʱ�ȭ

        // �� �̵�
        SceneLoader.instance.LoadScene((int)ESceneType.Game);
        SceneLoader.instance.LoadSceneAdditive((int)ESceneType.Crafting);
        SceneLoader.instance.LoadSceneAdditive((int)ESceneType.UI);
        SceneLoader.instance.LoadSceneAdditive((int)ESceneType.Map);
        App.instance.GetSoundManager().StopBGM();
    }

    public void Settings()
    {
        // ���� â
        Debug.Log("����");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
