using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    // Ʃ�丮�� �� ���� ��ũ��Ʈ

    [Header("Tutorial")]
    [SerializeField]  TutorialController tutorialController;

    public TutorialController GetTutorialController()
    {
        return tutorialController;
    }

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

        UIManager.instance.GetQuestController().StartMainQuest();

        Destroy(this);
    }
}
