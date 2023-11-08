using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Yarn.Unity;

public abstract class NotePageBase : MonoBehaviour
{
    protected List<string> todayNodeNames = new List<string>();
    protected List<string> tomorrowNodeNames = new List<string>();
    protected int index;
    protected ENotePageType eNotePageType;

    DialogueRunner m_dialogueRunner;
    VerticalLayoutGroup m_content;
    VerticalLayoutGroup m_lineView;

    public virtual ENotePageType GetENotePageType()
    {
        return eNotePageType;
    }

    public virtual void SetNodeName(string _nodeName)
    {
        tomorrowNodeNames.Add(_nodeName);
    }

    public virtual void InitNodeName()
    {
        todayNodeNames = tomorrowNodeNames;
        tomorrowNodeNames.Clear();
        index = 0;
    }

    public void PlayFirstNode()
    {
        PlayNode(todayNodeNames[0]);
    }

    public virtual void PlayPageAction(string _btnType)
    {
        if (index > todayNodeNames.Count - 1)
            index = todayNodeNames.Count - 1;
        else if (index < 0)
            index = 0;

        PlayNode(todayNodeNames[index]);

        if (_btnType == "next")
            index++;
        else
            index--;
    }

    public virtual void PlayNode(string _nodeName)
    {
        if (m_dialogueRunner.IsDialogueRunning == true)
            m_dialogueRunner.Stop();

        m_dialogueRunner.StartDialogue(_nodeName);
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_content.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_lineView.GetComponent<RectTransform>());

    }

    public virtual bool GetPageEnableToday()
    {
        if (todayNodeNames.Count > 0)
            return true;
        else return false;
    }
}
