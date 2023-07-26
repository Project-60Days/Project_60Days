using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    // 튜토리얼 씬 관리 스크립트

    [Header("Prefabs")]

    [Header("Tutorial")]
    [SerializeField] public TutorialController tutorialController;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        tutorialController.Init();
    }

    public void EndTutorial()
    {
        // 튜토리얼 끝
    }
}
