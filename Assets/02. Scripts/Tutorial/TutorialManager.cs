using System.Collections;
using UnityEngine;

public class TutorialManager : Manager
{
    [SerializeField] TutorialCtrl tutorialCtrl;

    public TutorialCtrl Ctrl => tutorialCtrl;

    private void Start()
    {
        StartCoroutine(WaitForMapManager());
    }

    private IEnumerator WaitForMapManager()
    {
        yield return new WaitUntil(() => App.Manager.Map.mapCtrl.Player != null);

        StartTutorial();
    }

    public void StartTutorial()
    {
        App.Manager.UI.GetPanel<PagePanel>().SetTutorialSelect();

        App.Manager.UI.GetPanel<CraftPanel>().Craft.AddBatteryCombine();

        App.Manager.UI.GetPanel<FixedPanel>().SetAlert(AlertType.Note, false);

        App.Manager.UI.GetPanel<InventoryPanel>().AddItemByItemCode("ITEM_PLASMA");
        App.Manager.UI.GetPanel<InventoryPanel>().AddItemByItemCode("ITEM_CARBON");
        App.Manager.UI.GetPanel<InventoryPanel>().AddItemByItemCode("ITEM_STEEL");

        Ctrl.StartDialogue();
    }

    public void EndTutorial()
    {
        App.Manager.UI.GetPanel<CraftPanel>().Craft.RemoveBatteryCombine();

        App.Manager.UI.GetPanel<QuestPanel>().StartQuest("MAIN_01");

        Destroy(this);
    }
}
