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

    bool isNeedToday;

    public override ENotePageType GetENotePageType()
    {
        return ENotePageType.DayStart;
    }

    public override void PlayPageAction()
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

    public override void SetPageEnabled(bool isNeedToday)
    {
        this.isNeedToday = isNeedToday;
    }

    public override bool GetPageEnabled()
    {
        return isNeedToday;
    }

    public override void StopDialogue()
    {
        dialogueRunner.Stop();
    }
}
