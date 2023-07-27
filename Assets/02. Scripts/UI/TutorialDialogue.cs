using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Yarn.Unity;
using static Yarn.Unity.Effects;
using DG.Tweening;

public class TutorialDialogue : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] DialogueRunner dialogueRunner;
    [SerializeField] GameObject ImageBack;
    [SerializeField] VerticalLayoutGroup content;
    [SerializeField] VerticalLayoutGroup lineView;
    [SerializeField] Button CloseBtn;

    private Coroutine runningCoroutine;
    private CoroutineInterruptToken interruptToken;

    public void Show()
    {
        Debug.Log("show");
        ImageBack.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0f, 0f), 1f);
    }

    public void Hide()
    {
        Debug.Log("hide");
        ImageBack.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-910f, 0f), 1f);
    }

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
