using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TutorialManager : Singleton<TutorialManager>
{
    // 튜토리얼 씬 관리 스크립트

    [Header("Tutorial")]
    [SerializeField]  TutorialController tutorialController;

    public Image lightBackground;
    public bool isLightUp = false;

    public TutorialController GetTutorialController()
    {
        return tutorialController;
    }

    void Awake()
    {
        lightBackground = GameObject.FindWithTag("LightImage").GetComponent<Image>();
        tutorialController.Init();
    }

    public void LightUpBackground()
    {
        lightBackground.DOFade(0f, 2f).SetEase(Ease.InBounce).OnComplete(() =>
        {
            lightBackground.gameObject.SetActive(false);
            isLightUp = true;
        });
    }

    public void EndTutorial()
    {
        Destroy(this);
    }
}
