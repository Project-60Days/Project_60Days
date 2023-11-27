using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class SelectPage : NotePageBase
{
    [SerializeField] DialogueRunner dialogueRunner;
    [SerializeField] RectTransform content;
    [SerializeField] RectTransform lineView;

    public override ENotePageType GetENotePageType()
    {
        return ENotePageType.Select;
    }

    public override void SetNodeName(string _nodeName)
    {
        tomorrowNodeNames.Add(_nodeName);
    }


    public override void PlayNode(string _nodeName)
    {
        if (dialogueRunner.IsDialogueRunning == true)
            dialogueRunner.Stop();

        dialogueRunner.StartDialogue(_nodeName);

        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        LayoutRebuilder.ForceRebuildLayoutImmediate(lineView);
    }
}
