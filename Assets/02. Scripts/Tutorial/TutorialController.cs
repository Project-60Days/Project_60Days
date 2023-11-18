using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Yarn.Unity;

public class TutorialController : MonoBehaviour
{
    [SerializeField] RectTransform tutorialBack;
    [SerializeField] Image whitePanel;
    [SerializeField] DialogueRunner dialogueRunner;

    Image lightBackground;
    Image workBenchImage;
    public bool isLightUp = false;

    int initIndex;

    void Awake()
    {
        lightBackground = GameObject.FindWithTag("LightImage").GetComponent<Image>();
        lightBackground.gameObject.SetActive(false);
        workBenchImage = GameObject.FindWithTag("WorkBench").GetComponent<Image>();

        initIndex = workBenchImage.transform.GetSiblingIndex();

        float newY = tutorialBack.rect.height * -2;
        tutorialBack.DOMove(new Vector2(0f, newY), 0f);
    }

    public void StartDialogue()
    {
        Show();
        dialogueRunner.StartDialogue("Tutorial_01");
    }

    public void Show()
    {
        tutorialBack.DOMove(new Vector2(0f, 0f), 0.3f).SetEase(Ease.InQuad);
        whitePanel.raycastTarget = true;
    }

    public void Hide()
    {
        float newY = tutorialBack.rect.height * -2;
        tutorialBack.DOMove(new Vector2(0f, newY), 0.3f).SetEase(Ease.OutQuad);
        whitePanel.raycastTarget = false;
    }

    

    public void LightUpWorkBench()
    {
        workBenchImage.transform.SetAsLastSibling();
        Color color = new Color(0.15f, 0.15f, 0.15f, 1f);
        workBenchImage.DOColor(color, 0f);
    }
    public void LightDownWorkBench()
    {
        workBenchImage.transform.SetSiblingIndex(initIndex);
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
