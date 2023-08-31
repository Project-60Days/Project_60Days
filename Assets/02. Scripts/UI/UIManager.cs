using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] NoteController noteController;
    [SerializeField] TutorialDialogue tutorialDialogue;
    [SerializeField] InventoryManager inventoryManager;
    [SerializeField] CraftingUiController craftingUIController;
    [SerializeField] UIHighLightController uiHighLightController;
    [SerializeField] EndUIController endUIController;

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

    public InventoryManager GetInventoryManager()
    {
        return inventoryManager;
    }

    public CraftingUiController GetCraftingUIController()
    {
        return craftingUIController;
    }

    public UIHighLightController GetUIHighLightController()
    {
        return uiHighLightController;
    }

    public EndUIController GetEndUIController()
    {
        return endUIController;
    }
}
