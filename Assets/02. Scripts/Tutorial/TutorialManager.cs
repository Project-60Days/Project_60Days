using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    // 튜토리얼 씬 관리 스크립트

    [Header("Prefabs")]

    [Header("Tutorial")]
    [SerializeField] TutorialController tutorialController;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        tutorialController.Init();
    }
}
