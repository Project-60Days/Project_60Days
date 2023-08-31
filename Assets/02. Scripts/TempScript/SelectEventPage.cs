using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

//public class SelectEventPage : NotePage
//{
//    [SerializeField] DialogueRunner dialogueRunner;
//    [SerializeField] VerticalLayoutGroup content;
//    [SerializeField] VerticalLayoutGroup lineView;

//    bool isNeedToday;

//    public override ENotePageType GetENotePageType()
//    {
//        return ENotePageType.SelectEvent;
//    }

//    public override void PlayPageAction()
//    {
//        GameManager.instance.SetPrioryty(false);
//        int dayCount = UIManager.instance.GetNoteController().GetDayCount();

//        string nodeName = "Day" + dayCount + "ChooseEvent";

//        if (!dialogueRunner.IsDialogueRunning)
//        {
//            dialogueRunner.StartDialogue(nodeName);
//            LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
//            LayoutRebuilder.ForceRebuildLayoutImmediate(lineView.GetComponent<RectTransform>());
//        }
//    }

//    public override void SetPageEnabled(bool isNeedToday)
//    {
//        this.isNeedToday = isNeedToday;
//    }

//    public override bool GetPageEnabled()
//    {
//        return true;
//    }

//    public override void StopDialogue()
//    {
//        dialogueRunner.Stop();
//    }
//}
