using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class SelectPage : PageBase
{
    [SerializeField] DialogueRunner dialogueRunner;
    [SerializeField] RectTransform content;
    [SerializeField] RectTransform lineView;

    public override PageType GetPageType()
    {
        return PageType.Select;
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

    public override void InitInChildren()
    {
        for (int i = 1; i < content.childCount; i++)
            if (content.GetChild(i).name == "SelectPage(Clone)")
                Destroy(content.GetChild(i).gameObject);
    }
}
