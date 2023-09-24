using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TutorialController : MonoBehaviour
{
    GameObject workBench;
    Image lightBackground;
    Image workBenchImage;
    public bool isLightUp = false;

    int initIndex;

    public void Show()
    {
        Debug.Log("TutorialUI Show");
        GetComponent<Transform>().DOMove(new Vector2(0f, 0f), 0.3f).SetEase(Ease.InQuad);
    }

    public void Hide()
    {
        Debug.Log("TutorialUI Hide");
        GetComponent<Transform>().DOMove(new Vector2(0f, -400f), 0.3f).SetEase(Ease.OutQuad);
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
}
