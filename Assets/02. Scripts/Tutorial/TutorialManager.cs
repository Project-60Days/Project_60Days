using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TutorialManager : Singleton<TutorialManager>
{
    // 튜토리얼 씬 관리 스크립트

    [Header("Tutorial")]
    [SerializeField]  TutorialController tutorialController;

    public Image lightBackground;
    public Image workBench;
    public bool isLightUp = false;

    public TutorialController GetTutorialController()
    {
        return tutorialController;
    }

    void Awake()
    {
        lightBackground = GameObject.FindWithTag("LightImage").GetComponent<Image>();
        workBench = GameObject.FindWithTag("WorkBench").GetComponent<Image>();
    }

    public void LightUpWorkBench()
    {
        Color color = new Color(0.15f, 0.15f, 0.15f, 1f);
        workBench.DOColor(color, 0.5f);
        Debug.Log("LightUp");
    }
    public void LightDownWorkBench()
    {
        Color color = new Color(0.020f, 0.020f, 0.020f, 1f);
        workBench.DOColor(color, 0f);
        Debug.Log("LightDown");
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
