using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class SelectPage : NotePageBase
{
    [SerializeField] DialogueRunner dialogueRunner;
    [SerializeField] VerticalLayoutGroup content;
    [SerializeField] VerticalLayoutGroup lineView;

    bool isNeedToday = true; //임시로 true를 default로 설정
    string nodeName;

    public override ENotePageType GetENotePageType()
    {
        return ENotePageType.Select;
    }

    public override void PlayPageAction()
    {
        nodeName = "Select"; //임시로 노드 이름 설정

        if (dialogueRunner.IsDialogueRunning == false)
        {
            dialogueRunner.StartDialogue(nodeName);
            LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(lineView.GetComponent<RectTransform>());
        }
    }

    public override void SetNodeName(string _nodeName)
    {
        this.nodeName = _nodeName;
    }

    public override void SetPageEnabled(bool _isNeedToday)
    {
        this.isNeedToday = _isNeedToday;
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
