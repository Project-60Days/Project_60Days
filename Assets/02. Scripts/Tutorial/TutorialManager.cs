using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using static Yarn.Unity.Effects;
using Yarn.Unity;

public class TutorialManager : Singleton<TutorialManager>
{
    // 튜토리얼 씬 관리 스크립트

    [Header("Tutorial")]
    [SerializeField]  TutorialController tutorialController;

    public TutorialController GetTutorialController()
    {
        return tutorialController;
    }

    public void StartTutorial()
    {
        
    }

    public void EndTutorial()
    {
        Destroy(this);
    }
}
