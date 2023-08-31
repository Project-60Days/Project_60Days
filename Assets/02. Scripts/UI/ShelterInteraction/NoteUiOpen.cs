using UnityEngine;

public class NoteUiOpen : MonoBehaviour
{
    private void OnEnable()
    {
        SetOnClickEvent(true);
    }

    private void OnDisable()
    {
        SetOnClickEvent(false);
    }

    private void SetOnClickEvent(bool enable)
    {
        NoteInteraction onClickScript = FindObjectOfType<NoteInteraction>();
        if (onClickScript != null)
        {
            if (enable)
            {
                onClickScript.onClickEvent.AddListener(ActivateUIObjects);
            }
            else
            {
                onClickScript.onClickEvent.RemoveListener(ActivateUIObjects);
            }
        }
    }

    private void ActivateUIObjects()
    {
        UIManager.instance.GetNoteController().OpenNote();
    }
}
