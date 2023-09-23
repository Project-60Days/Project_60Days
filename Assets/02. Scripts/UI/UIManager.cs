using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] NoteController noteController;
    [SerializeField] TutorialDialogue tutorialDialogue;
    [SerializeField] InventoryController inventoryController;
    [SerializeField] CraftingUiController craftingUiController;
    [SerializeField] CraftingRawImageController craftingRawImageController;
    [SerializeField] CraftModeController craftModeController;
    [SerializeField] UIHighLightController uiHighLightController;
    [SerializeField] SelectController selectController;
    [SerializeField] NextDayController nextDayController;

    public Stack<string> currUIStack = new Stack<string>();

    private void Awake()
    {
        currUIStack.Push(StringUtility.UI_NORMAL);
    }

    public void AddCurrUIName(string _uiName)
    {
        currUIStack.Push(_uiName);

        Debug.LogError("currUIStack : " + currUIStack.Peek());
    }

    public void PopCurrUI()
    {
        currUIStack.Pop();
    }

    public bool isUIStatus(string _cmp)
    {
        currUIStack.TryPeek(out string top);
        return _cmp == top;
    }

    public NoteController GetNoteController()
    {
        return noteController;
    }

    public TutorialDialogue GetTutorialDialogue()
    {
        return tutorialDialogue;
    }

    public InventoryController GetInventoryController()
    {
        return inventoryController;
    }

    public CraftingUiController GetCraftingUiController()
    {
        return craftingUiController;
    }

    public CraftingRawImageController GetCraftingRawImageController()
    {
        return craftingRawImageController;
    }

    public CraftModeController GetCraftModeController()
    {
        return craftModeController;
    }
   
    public UIHighLightController GetUIHighLightController()
    {
        return uiHighLightController;
    }

    public SelectController GetSelectController()
    {
        return selectController;
    }

    public NextDayController GetNextDayController()
    {
        return nextDayController;
    }
}
