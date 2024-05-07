using System.Collections;
using UnityEngine;

public class TutorialManager : Manager
{
    [SerializeField] TutorialCtrl tutorialCtrl;

    public TutorialCtrl Ctrl => tutorialCtrl;

    private UIManager UI;

    private void Start()
    {
        UI = App.Manager.UI;

        if (App.Manager.Game.startTutorial) 
            StartCoroutine(WaitForLoad());
    }

    private IEnumerator WaitForLoad()
    {
        yield return new WaitUntil(() => App.Manager.Map.GetUnit<PlayerUnit>().player != null);

        StartTutorial();
    }

    public void StartTutorial()
    {
        App.Manager.Shelter.StartTutorial();

        UI.GetPanel<PagePanel>().SetTutorialSelect();
        UI.GetPanel<FixedPanel>().SetAlert(AlertType.Note, false);

        UI.GetPanel<CraftPanel>().Craft.AddBatteryCombine();

        UI.GetPanel<InventoryPanel>().AddItemByItemCode("ITEM_PLASMA", "ITEM_CARBON", "ITEM_STEEL");

        Ctrl.StartDialogue();
    }

    public void EndTutorial()
    {
        UI.GetPanel<CraftPanel>().Craft.RemoveBatteryCombine();

        UI.GetPanel<QuestPanel>().StartQuest("MAIN_01");

        Destroy(this);
    }
}
