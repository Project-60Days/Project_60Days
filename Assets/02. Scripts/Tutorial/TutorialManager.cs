using System.Collections;
using UnityEngine;

public class TutorialManager : Manager
{
    [Header("Tutorial")]
    [SerializeField]  TutorialController tutorialController;

    public TutorialController GetTutorialController() => tutorialController;

    void Start()
    {
        StartCoroutine(WaitForMapManager());
    }

    IEnumerator WaitForMapManager()
    {
        yield return new WaitUntil(() => App.Manager.Map.mapController);
        yield return new WaitUntil(() => App.Manager.Map.mapController.Player != null);

        StartTutorial();
    }

    public void StartTutorial()
    {
        App.Manager.UI.GetPageController().SetTutorialSelect();

        App.Manager.UI.GetCraftingUiController().AddBatteryCombine();

        App.Manager.UI.GetAlertController().SetAlert("note", false);

        App.Manager.UI.GetPanel<InventoryPanel>().AddItemByItemCode("ITEM_PLASMA");
        App.Manager.UI.GetPanel<InventoryPanel>().AddItemByItemCode("ITEM_CARBON");
        App.Manager.UI.GetPanel<InventoryPanel>().AddItemByItemCode("ITEM_STEEL");

        tutorialController.StartDialogue();
    }

    public void EndTutorial()
    {
        App.Manager.UI.GetCraftingUiController().RemoveBatteryCombine();

        App.Manager.UI.GetQuestController().StartMainQuest();

        Destroy(this);
    }
}
