using UnityEngine;

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
        tutorialController.LightDownBackground();
    }

    public void EndTutorial()
    {
        Destroy(this);
    }
}
