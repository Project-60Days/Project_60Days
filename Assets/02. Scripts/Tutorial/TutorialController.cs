using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Yarn.Unity;
using System.Collections;

public class TutorialController : MonoBehaviour
{
    [SerializeField] RectTransform tutorialBack;
    [SerializeField] Image whitePanel;
    [SerializeField] DialogueRunner dialogueRunner;
    [SerializeField] Selectable skipButton;

    Image lightBackground;
    Image workBenchImage;
    Image batteryImage;

    float lightUpDuration = 2f;

    public bool isLightUp = false;

    int initIndex;

    void Awake()
    {
        lightBackground = GameObject.FindWithTag("LightImage").GetComponent<Image>();
        lightBackground.gameObject.SetActive(false);
        workBenchImage = GameObject.FindWithTag("WorkBench").GetComponent<Image>();
        batteryImage = GameObject.FindWithTag("Battery").GetComponent<Image>();

        initIndex = workBenchImage.transform.GetSiblingIndex();

        float newY = tutorialBack.rect.height * -2;
        tutorialBack.DOMove(new Vector2(0f, newY), 0f);
    }

    void Update()
    {
        skipButton.Select();
    }

    public void StartDialogue()
    {
        LightDownBackground();
        Color color = new Color(0.15f, 0.15f, 0.15f, 1f);
        workBenchImage.DOColor(color, 0f);
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
    }
    public void LightDownWorkBench()
    {
        workBenchImage.transform.SetSiblingIndex(initIndex);
        Color color = new Color(1f, 1f, 1f, 1f);
        workBenchImage.DOColor(color, 0f);
    }
    public void LightUpBackground()
    {
        StartCoroutine(FillBattery());
        lightBackground.DOFade(0f, lightUpDuration).SetEase(Ease.InBounce).OnComplete(() =>
        {
            lightBackground.gameObject.SetActive(false);
            isLightUp = true;
        });
    }

    IEnumerator FillBattery()
    {
        float timer = 0f;
        float currentFill;
        float targetFill = 1f; // 이미지를 채울 양 (0 ~ 1)

        while (timer < lightUpDuration)
        {
            timer += Time.deltaTime;
            currentFill = Mathf.Lerp(0f, targetFill, timer / lightUpDuration);
            batteryImage.fillAmount = currentFill;

            yield return null;
        }

        batteryImage.fillAmount = targetFill;
    }

    void LightDownBackground()
    {
        lightBackground.DOFade(1f, 0f).OnComplete(() =>
        {
            lightBackground.gameObject.SetActive(true);
            batteryImage.fillAmount = 0;
            isLightUp = false;
        });
    }
}
