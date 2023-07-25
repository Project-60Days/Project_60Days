using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] NoteController noteController;
    [SerializeField] TutorialDialogue tutorialDialogue;
    [SerializeField] InventoryManager inventoryManager;
    [SerializeField] CraftingUIController craftingUIController;
    [SerializeField] UIHighLightController uiHighLightController;

    public Stack<string> currUIStack;

    public void AddCurrUIName(string _uiName)
    {
        currUIStack.Push(_uiName);
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

    public CraftingUIController GetCraftingUIController()
    {
        return craftingUIController;
    }

    public UIHighLightController GetUIHighLightController()
    {
        return uiHighLightController;
    }
}
