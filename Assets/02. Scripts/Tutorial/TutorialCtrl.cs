using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Yarn.Unity;

public class TutorialCtrl : MonoBehaviour
{
    [SerializeField] RectTransform tutorialBack;
    [SerializeField] Image whitePanel;
    [SerializeField] DialogueRunner dialogueRunner;

    private void Start()
    {
        Hide();
    }

    public void StartDialogue()
    {
        dialogueRunner.StartDialogue("Tutorial_01");
        Show();
    }

    public void Show()
    {
        tutorialBack.DOMoveY(0f, 0.3f).SetEase(Ease.InQuad);
        whitePanel.raycastTarget = true;
    }

    public void Hide()
    {
        float yPos = -tutorialBack.rect.height;
        tutorialBack.DOMoveY(yPos, 0.3f).SetEase(Ease.OutQuad);
        whitePanel.raycastTarget = false;
    }

    public void AddResource()
    {
        string nodeName = "ITEM_TUTORIAL";
        App.Manager.UI.GetPanel<PagePanel>().SetResultPage(nodeName, true);

        App.Manager.UI.GetPanel<InventoryPanel>().AddItemByItemCode("ITEM_DISTURBE", "ITEM_FINDOR");
    }
}
