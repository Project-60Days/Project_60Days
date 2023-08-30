using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class SelectEventPage : NotePage
{
    [SerializeField] DialogueRunner dialogueRunner;
    [SerializeField] VerticalLayoutGroup content;
    [SerializeField] VerticalLayoutGroup lineView;

    public override ENotePageType GetENotePageType()
    {
        return ENotePageType.SelectEvent;
    }

    public override int GetPriority()
    {
        return 3;
    }

    public override void playPageAction()
    {
        int dayCount = UIManager.instance.GetNoteController().GetDayCount();

        string nodeName = "Day" + dayCount + "ChooseEvent";

        if (!dialogueRunner.IsDialogueRunning)
        {
            dialogueRunner.StartDialogue(nodeName);
            LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(lineView.GetComponent<RectTransform>());
        }
    }

    public void StopDialogue()
    {
        dialogueRunner.Stop();
    }
}
