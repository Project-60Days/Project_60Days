using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TutorialController : MonoBehaviour
{
    [SerializeField] Transform tutorialBack;
    [SerializeField] Image whitePanel;

    GameObject workBench;
    Image lightBackground;
    Image workBenchImage;
    public bool isLightUp = false;

    int initIndex;

    void Awake()
    {
        lightBackground = GameObject.FindWithTag("LightImage").GetComponent<Image>();
        workBench = GameObject.FindWithTag("WorkBench");
        workBenchImage = workBench.GetComponent<Image>();

        initIndex = workBench.transform.GetSiblingIndex();
    }

    public void Show()
    {
        tutorialBack.DOMove(new Vector2(0f, 0f), 0.3f).SetEase(Ease.InQuad);
        whitePanel.raycastTarget = true;
    }

    public void Hide()
    {
        tutorialBack.DOMove(new Vector2(0f, -400f), 0.3f).SetEase(Ease.OutQuad);
        whitePanel.raycastTarget = false;
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
        lightBackground.DOFade(0f, 2f).SetEase(Ease.InBounce).OnComplete(() =>
        {
            lightBackground.gameObject.SetActive(false);
            isLightUp = true;
        });
    }
    public void LightDownBackground()
    {
        lightBackground.DOFade(1f, 0f).SetEase(Ease.InBounce).OnComplete(() =>
        {
            lightBackground.gameObject.SetActive(true);
            isLightUp = false;
        });
    }
}
