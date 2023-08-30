using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class StoryPage : NotePage
{
    [SerializeField] DialogueRunner dialogueRunner;
    [SerializeField] VerticalLayoutGroup content;
    [SerializeField] VerticalLayoutGroup lineView;

    public override ENotePageType GetENotePageType()
    {
        return ENotePageType.DayStart;
    }

    public override int GetPriority()
    {
        return 1;
    }

    public override void playPageAction()
    {
        int dayCount = UIManager.instance.GetNoteController().GetDayCount();

        string nodeName = "Day" + dayCount;

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
