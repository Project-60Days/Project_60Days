using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Yarn.Unity;
using System.Collections;

public class TutorialCtrl : MonoBehaviour
{
    [SerializeField] RectTransform tutorialBack;
    [SerializeField] Image whitePanel;
    [SerializeField] DialogueRunner dialogueRunner;
    [SerializeField] Selectable skipButton;

    [SerializeField] Button nextDayBtn;
    [SerializeField] Button backToBaseBtn;

    [SerializeField] CustomDialogueView customView;

    private void Start()
    {
        float newY = tutorialBack.rect.height * -2; //TODO
        tutorialBack.DOMove(new Vector2(0f, newY), 0f);
    }

    private void Update()
    {
        skipButton.Select();

        if (Input.GetMouseButtonDown(0))
            customView.UserRequestedViewAdvancement();
    }

    public void StartDialogue()
    {
        App.Manager.Shelter.StartTutorial();
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
        float newY = tutorialBack.rect.height * -2; //TODO
        tutorialBack.DOMove(new Vector2(0f, newY), 0.3f).SetEase(Ease.OutQuad);
        whitePanel.raycastTarget = false;
    }

    public void EnableBtn()
    {
        backToBaseBtn.enabled = true;
        nextDayBtn.enabled = true;
    }

    public void AddSteel()
    {
        string nodeName = "ITEM_TUTORIAL";

        App.Manager.UI.GetPanel<PagePanel>().SetResultPage(nodeName, true);

        StartCoroutine(AddResource());
    }

    private IEnumerator AddResource()
    {
        yield return new WaitUntil(() => App.Manager.UI.CurrState == UIState.Normal);

        App.Manager.UI.GetPanel<InventoryPanel>().AddItemByItemCode("ITEM_DISTURBE");
        App.Manager.UI.GetPanel<InventoryPanel>().AddItemByItemCode("ITEM_FINDOR");
    }
}
