using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void UpdateAllState()
    {
        // 게임 시작 시 모든 데이터 연결

        // 1. 일차 업데이트

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
        SceneManager.LoadScene("02. GameScene");
    }

    public void NewGameStart()
    {
        // 데이터 초기화

        // 씬 이동
        SceneManager.LoadScene("02. GameScene");
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
