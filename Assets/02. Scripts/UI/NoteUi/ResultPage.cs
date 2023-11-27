using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Yarn.Unity;

public class ResultPage : NotePageBase
{
    [SerializeField] DialogueRunner dialogueRunner;
    [SerializeField] VerticalLayoutGroup content;
    [SerializeField] Transform prefabParent;

    List<string> tomorrowResourceNodeNames = new List<string>();

    public override ENotePageType GetENotePageType()
    {
        return ENotePageType.Result;
    }

    public override void PlayNode(string _nodeName)
    {
        resourceIndex = 0;

        for (int i = 1; i < prefabParent.childCount; i++)
            if(prefabParent.GetChild(i).name == "ResultPage(Clone)")
                Destroy(prefabParent.GetChild(i).gameObject);

        if (dialogueRunner.IsDialogueRunning == true)
            dialogueRunner.Stop();

        dialogueRunner.StartDialogue(_nodeName);
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
    }

    public override void SetNodeName(string _nodeName, bool _isResourceNode)
    {
        if (_isResourceNode == true)
            tomorrowResourceNodeNames.Add(_nodeName);
        else
            tomorrowNodeNames.Add(_nodeName);
    }

    public override void InitResourceNodeName()
    {
        todayResourceNodeNames.Clear();

        for (int i = 0; i < tomorrowResourceNodeNames.Count; i++)
            todayResourceNodeNames.Add(tomorrowResourceNodeNames[i]);

        tomorrowResourceNodeNames.Clear();

        if (todayResourceNodeNames.Count > 0)
            todayNodeNames.Add(todayResourceNodeNames[0]);
    }
}
