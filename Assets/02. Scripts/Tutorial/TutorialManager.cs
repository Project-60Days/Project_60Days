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

    void Awake()
    {
        UIManager.instance.GetPageController().SetTutorialSelect();
    }

    void Start()
    {
        UIManager.instance.GetAlertController().SetAlert("note", false);
        UIManager.instance.GetInventoryController().AddItemByItemCode("ITEM_PLASMA");
        UIManager.instance.GetInventoryController().AddItemByItemCode("ITEM_CARBON");
        UIManager.instance.GetInventoryController().AddItemByItemCode("ITEM_STEEL");
        
    }

    public void StartTutorial()
    {
        tutorialController.LightDownBackground();
    }

    public void EndTutorial()
    {
        Destroy(this);
    }
}
