using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] NoteController noteController;
    [SerializeField] TutorialDialogue tutorialDialogue;
    [SerializeField] InventoryManager inventoryManager;
    [SerializeField] CraftingUIController craftingUIController;

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
}
