using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class SelectPage : NotePage
{
    [SerializeField] DialogueRunner dialogueRunner;
    [SerializeField] VerticalLayoutGroup content;
    [SerializeField] VerticalLayoutGroup lineView;

    bool isNeedToday = true;
    string nodeName;

    public override ENotePageType GetENotePageType()
    {
        return ENotePageType.Select;
    }

    public override void PlayPageAction()
    {
        //int dayCount = UIManager.instance.GetNoteController().GetDayCount();
        //string nodeName = "Day" + dayCount;

        nodeName = "Select";

        if (!dialogueRunner.IsDialogueRunning)
        {
            dialogueRunner.StartDialogue(nodeName);
            LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(lineView.GetComponent<RectTransform>());
        }
    }
    public override void SetNodeName(string nodeName)
    {
        this.nodeName = nodeName;
    }

    public override void SetPageEnabled(bool isNeedToday)
    {
        this.isNeedToday = isNeedToday;
    }

    public override bool GetPageEnableToday()
    {
        return isNeedToday;
    }

    public override void StopDialogue()
    {
        dialogueRunner.Stop();
    }
}
