using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    // 튜토리얼 씬 관리 스크립트

    [Header("Tutorial")]
    [SerializeField]  TutorialController tutorialController;

    public TutorialController GetTutorialController()
    {
        return tutorialController;
    }

    public void StartTutorial()
    {
        UIManager.instance.GetPageController().SetTutorialSelect();

        UIManager.instance.GetAlertController().SetAlert("note", false);
        UIManager.instance.GetInventoryController().AddItemByItemCode("ITEM_PLASMA");
        UIManager.instance.GetInventoryController().AddItemByItemCode("ITEM_CARBON");
        UIManager.instance.GetInventoryController().AddItemByItemCode("ITEM_STEEL");

        tutorialController.StartDialogue();
    }

    public void EndTutorial()
    {
        Destroy(this);
    }
}
