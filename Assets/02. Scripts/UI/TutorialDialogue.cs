using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Yarn.Unity;
using static Yarn.Unity.Effects;
using DG.Tweening;

public class TutorialDialogue : MonoBehaviour
{
    [SerializeField] DialogueRunner dialogueRunner;
    [SerializeField] GameObject imageBack;
    [SerializeField] VerticalLayoutGroup content;
    [SerializeField] VerticalLayoutGroup lineView;

    private Coroutine runningCoroutine;
    private CoroutineInterruptToken interruptToken;

    public void Show()
    {
        Debug.Log("TutorialUI Show");
        imageBack.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0f, 0f), 0.3f).SetEase(Ease.InQuad);
    }

    public void Hide()
    {
        Debug.Log("TutorialUI Hide");
        imageBack.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0f, -400f), 0.3f).SetEase(Ease.OutQuad);
    }

    public void StartDialogue(string _nodeName)
    {
        if (dialogueRunner.IsDialogueRunning == true) dialogueRunner.Stop();

        dialogueRunner.StartDialogue(_nodeName);
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(lineView.GetComponent<RectTransform>());

        interruptToken = new CoroutineInterruptToken(); //?
    }

    public void EndDialogue()
    {
        dialogueRunner.Stop();
        TutorialManager.instance.GetTutorialController().SetNextTutorial();
    }
}
