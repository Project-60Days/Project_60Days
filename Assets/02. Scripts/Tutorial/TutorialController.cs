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

    [SerializeField] Button nextDayBtn;
    [SerializeField] Button backToBaseBtn;

    Image lightBackground;
    CanvasGroup workBenchImage;
    Image batteryImage;
    CanvasGroup mapImage;
    CanvasGroup workBenchDeco;

    WorkBenchInteraction workBenchScript;

    [SerializeField] CustomDialogueView customView;

    float lightUpDuration = 2f;

    int workBenchInitIndex;
    int mapInitIndex;

    void Awake()
    {
        lightBackground = GameObject.FindWithTag("LightImage").GetComponent<Image>();
        lightBackground.gameObject.SetActive(false);
        workBenchImage = GameObject.FindWithTag("WorkBench").GetComponent<CanvasGroup>();
        batteryImage = GameObject.FindWithTag("Battery").GetComponent<Image>();
        mapImage = GameObject.FindWithTag("Map").GetComponent<CanvasGroup>();
        workBenchDeco = GameObject.FindWithTag("WorkBenchDeco").GetComponent<CanvasGroup>();
        workBenchScript = workBenchImage.GetComponent<WorkBenchInteraction>();

        workBenchInitIndex = workBenchImage.transform.GetSiblingIndex();
        mapInitIndex = workBenchImage.transform.GetSiblingIndex();

        float newY = tutorialBack.rect.height * -2;
        tutorialBack.DOMove(new Vector2(0f, newY), 0f);
    }

    void Update()
    {
        skipButton.Select();

        if (Input.GetMouseButtonDown(0))
            customView.UserRequestedViewAdvancement();
    }

    public void StartDialogue()
    {
        LightDownBackground();

        workBenchDeco.DOFade(0f, 0f);
        Show();
        dialogueRunner.StartDialogue("Tutorial_01");

        backToBaseBtn.enabled = false;
        nextDayBtn.enabled = false;
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

    public void LightUpAndFillBattery(int _num)
    {
        float alpha = 0.25f * _num;

        StartCoroutine(FillBattery(alpha));

        lightBackground.DOFade(1 - alpha, lightUpDuration).SetEase(Ease.InBounce);
    }



    public void LightUpWorkBench()
    {
        workBenchImage.DOFade(0.4f, 0f).OnComplete(() => workBenchImage.transform.SetAsLastSibling());
    }

    public void LightDownWorkBench()
    {
        workBenchImage.transform.SetSiblingIndex(workBenchInitIndex);

        workBenchImage.DOFade(1f, 0f);
    }

    public void LightUpMap()
    {
        mapImage.DOFade(0.6f, 0f).OnComplete(() => mapImage.transform.SetAsLastSibling());
    }
    public void LightDownMap()
    {
        mapImage.transform.SetSiblingIndex(mapInitIndex);

        mapImage.DOFade(1f, 0f);
    }

    public void LightUpBackground()
    {
        StartCoroutine(FillBattery(1f));
        
        lightBackground.DOFade(0f, lightUpDuration).OnComplete(() =>
        {
            workBenchDeco.DOFade(1f, lightUpDuration).SetEase(Ease.InOutBounce).OnComplete(() =>
            {
                workBenchScript.StartAnim();
            });
            lightBackground.gameObject.SetActive(false);
        });
    }

    IEnumerator FillBattery(float _amount)
    {
        float timer = 0f;
        float initFill = batteryImage.fillAmount;
        float currentFill;
        float targetFill = _amount;

        while (timer < lightUpDuration)
        {
            timer += Time.deltaTime;
            currentFill = Mathf.Lerp(initFill, targetFill, timer / lightUpDuration);
            batteryImage.fillAmount = currentFill;

            yield return null;
        }

        batteryImage.fillAmount = targetFill;
    }

    void LightDownBackground()
    {
        lightBackground.DOFade(0.95f, 0f).OnComplete(() =>
        {
            lightBackground.gameObject.SetActive(true);
            batteryImage.fillAmount = 0f;
        });
    }

    public void EnableBtn()
    {
        backToBaseBtn.enabled = true;
        nextDayBtn.enabled = true;
    }

    public void AddSteel()
    {
        string nodeName = "ITEM_TUTORIAL";

        UIManager.instance.GetPageController().SetResultPage(nodeName, true);

        StartCoroutine(AddResource());
    }

    IEnumerator AddResource()
    {
        yield return new WaitUntil(() => UIManager.instance.isUIStatus("UI_NORMAL"));

        UIManager.instance.GetInventoryController().AddItemByItemCode("ITEM_DISTURBE");
        UIManager.instance.GetInventoryController().AddItemByItemCode("ITEM_FINDOR");
    }
}
