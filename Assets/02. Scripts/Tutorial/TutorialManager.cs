using System.Collections;
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

    void Start()
    {
        //StartCoroutine(WaitForMapManager());
    }

    IEnumerator WaitForMapManager()
    {
        yield return new WaitUntil(() => App.instance.GetMapManager().Controller);
        yield return new WaitUntil(() => App.instance.GetMapManager().Controller.Player != null);

        StartTutorial();
    }

    public void StartTutorial()
    {
        UIManager.instance.GetPageController().SetTutorialSelect();

        UIManager.instance.GetCraftingUiController().AddBatteryCombine();

        UIManager.instance.GetAlertController().SetAlert("note", false);

        UIManager.instance.GetInventoryController().AddItemByItemCode("ITEM_PLASMA");
        UIManager.instance.GetInventoryController().AddItemByItemCode("ITEM_CARBON");
        UIManager.instance.GetInventoryController().AddItemByItemCode("ITEM_STEEL");

        tutorialController.StartDialogue();
    }

    public void EndTutorial()
    {
        UIManager.instance.GetCraftingUiController().RemoveBatteryCombine();

        UIManager.instance.GetQuestController().CreateQuest("chapter01_GetNetworkChip");

        Destroy(this);
    }
}
