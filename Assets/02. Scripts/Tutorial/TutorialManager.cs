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
        UIManager.instance.GetQuestController().CreateQuest("chapter01_GetNetworkChip"); //일단 임시.. pv나오면 그거 재생 끝나고 생성되게 수정하겠습니다.
        Destroy(this);
    }
}
