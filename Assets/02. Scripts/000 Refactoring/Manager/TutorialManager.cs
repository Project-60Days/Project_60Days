using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Yarn.Unity;

public class TutorialManager : Manager, IListener
{
    [SerializeField] RectTransform tutorialBack;
    [SerializeField] Image whitePanel;
    [SerializeField] DialogueRunner dialogueRunner;

    private float startPositionY;

    protected override void Awake()
    {
        base.Awake();

        App.Manager.Event.AddListener(EventCode.TutorialStart, this);
        App.Manager.Event.AddListener(EventCode.TutorialEnd, this);
    }

    public void OnEvent(EventCode _code, Component _sender, object _param = null)
    {
        switch (_code)
        {
            case EventCode.TutorialStart:
                StartDialogue();
                break;

            case EventCode.TutorialEnd:
                Destroy(gameObject);
                break;
        }
    }

    private void StartDialogue()
    {
        startPositionY = -tutorialBack.rect.height;

        dialogueRunner.StartDialogue("Tutorial_01");

        Show();
    }

    public void Show()
    {
        tutorialBack.DOKill();
        tutorialBack.DOAnchorPosY(0f, 0.3f).SetEase(Ease.InQuad);
        whitePanel.raycastTarget = true;
    }

    public void Hide()
    {
        tutorialBack.DOKill();
        tutorialBack.DOAnchorPosY(startPositionY, 0.3f).SetEase(Ease.OutQuad);
        whitePanel.raycastTarget = false;
    }

    public void AddResource()
    {
        string nodeName = "ITEM_TUTORIAL";
        App.Manager.UI.GetPanel<PagePanel>().SetResultPage(nodeName, true);

        App.Manager.UI.GetPanel<InventoryPanel>().AddItemByItemCode("ITEM_DISTURBE", "ITEM_FINDOR");
    }
}
