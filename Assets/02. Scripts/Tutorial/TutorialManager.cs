using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TutorialManager : Singleton<TutorialManager>
{
    // 튜토리얼 씬 관리 스크립트

    [Header("Tutorial")]
    [SerializeField]  TutorialController tutorialController;

    GameObject workBench;
    Image lightBackground;
    Image workBenchImage;
    public bool isLightUp = false;

    int initIndex;
    public TutorialController GetTutorialController()
    {
        return tutorialController;
    }

    void Awake()
    {
        lightBackground = GameObject.FindWithTag("LightImage").GetComponent<Image>();
        workBench = GameObject.FindWithTag("WorkBench");
        workBenchImage = workBench.GetComponent<Image>();

        initIndex = workBench.transform.GetSiblingIndex();
    }

    public void LightUpWorkBench()
    {
        workBench.transform.SetAsLastSibling();
        Color color = new Color(0.15f, 0.15f, 0.15f, 1f);
        workBenchImage.DOColor(color, 0f);
    }
    public void LightDownWorkBench()
    {
        workBench.transform.SetSiblingIndex(initIndex);
        Color color = new Color(1f, 1f, 1f, 1f);
        workBenchImage.DOColor(color, 0f);
    }
    public void LightUpBackground()
    {
        Color color = new Color(1f, 1f, 1f, 1f);
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
