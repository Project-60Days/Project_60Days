using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct Controller
{
    bool isGameOver;
}

/// <summary>
/// 일 수가 넘어감에 따라 맞게 맵, 캐릭터, 태스크 업데이트. -> 각 매니저에게 명령.
/// UI, 대화 시스템 컨트롤.
/// 게임 시작 및 종료, 게임 오버, 저장 구현.
/// 씬 관리.
/// </summary>
public class GameManager : Singleton<GameManager>
{
    Controller controller;
    MapCamera mapCamera;

    [SerializeField] GameObject tutorialManager;
    
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
        if (Input.GetKeyDown(KeyCode.Escape))
            QuitGame();
    }

    public void UpdateAllState()
    {
        // 1. 일차 변경

        // 2. 맵 업데이트

        // 3. 캐릭터 상태 업데이트

        // 4. 테스크 업데이트
    }

    public void SaveGame()
    {
        // 현재 모든 데이터 저장
    }

    public void PrevGameStart()
    {
        // 이전 데이터 불러오기

        // 씬 이동
        SceneLoader.instance.LoadScene(1);
    }

    public void NewGameStart()
    {
        // 데이터 초기화

        // 씬 이동
        SceneLoader.instance.LoadScene((int)ESceneType.Game);
        SceneLoader.instance.LoadSceneAddtive((int)ESceneType.UI);
        SceneLoader.instance.LoadSceneAddtive((int)ESceneType.Map);

        StartCoroutine(GetMapCamera());
        
    }

    private IEnumerator GetMapCamera()
    {
        yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("MapCamera") != null);

        mapCamera = GameObject.FindGameObjectWithTag("MapCamera").GetComponent<MapCamera>();

        StartTutorial();

        SoundManager.instance.PlayBGM("BGM_InGameTheme");
    }

    public void Settings()
    {
        // 설정 창
        Debug.Log("설정");
    }

    public void SetPrioryty(bool set)
    {
        mapCamera.SetPrioryty(set);
    }

    public void StartTutorial()
    {
        TutorialManager tm = Instantiate(tutorialManager).GetComponent<TutorialManager>();
        tm.lightBackground = GameObject.Find("LightBackground").GetComponent<Image>();
        DontDestroyOnLoad(tm.gameObject);
        tm.Init();
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
