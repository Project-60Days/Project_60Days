using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Yarn.Unity;
using static Yarn.Unity.Effects;

public class TutorialDialogue : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] DialogueRunner dialogueRunner;
    [SerializeField] VerticalLayoutGroup content;
    [SerializeField] VerticalLayoutGroup lineView;
    [SerializeField] Button CloseBtn;

    private Coroutine runningCoroutine;
    private CoroutineInterruptToken interruptToken;

    public void StartDialogue(string _nodeName)
    {
        this.gameObject.SetActive(true);

        dialogueRunner.Stop();

        CloseBtn.onClick.AddListener(EndDialogue);

        if (!dialogueRunner.IsDialogueRunning)
        {
            dialogueRunner.StartDialogue(_nodeName);
            LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(lineView.GetComponent<RectTransform>());
        }

        interruptToken = new CoroutineInterruptToken();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void EndDialogue()
    {
        this.gameObject.SetActive(false);
    }
}
