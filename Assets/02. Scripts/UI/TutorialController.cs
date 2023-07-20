using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Yarn.Unity;
using static Yarn.Unity.Effects;

public class TutorialController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] DialogueRunner dialogueRunner;
    [SerializeField] VerticalLayoutGroup content;
    [SerializeField] VerticalLayoutGroup lineView;

    private Coroutine runningCoroutine;
    private CoroutineInterruptToken interruptToken;

    void Start()
    {
        if (!dialogueRunner.IsDialogueRunning)
        {
            dialogueRunner.StartDialogue("Tutorial");
            LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(lineView.GetComponent<RectTransform>());
        }
        

        interruptToken = new CoroutineInterruptToken();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }
}
