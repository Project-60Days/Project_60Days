using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using TMPro;
using System;

public class CustomDialogueView : DialogueViewBase
{
    public Button[] skipButton;
    [SerializeField] private TextMeshProUGUI lineText;
    [SerializeField] private string skipSFX = null;
    [SerializeField] private string textSFX = null;

    //private bool isRunningLine = false;
    private bool doesUserContinueRequest = false;
    private bool doesUserSkipRequest = false;
    private bool isStartLine = false;

    Effects.CoroutineInterruptToken typingCoroutineStopToken = new Effects.CoroutineInterruptToken();

    // ������ ��ŵ ��ư ������ �� ȣ��Ǵ� �Լ�
    public override void UserRequestedViewAdvancement()
    {
        if (!doesUserSkipRequest)
        {
            doesUserSkipRequest = true;

            if (typingCoroutineStopToken.CanInterrupt)
                typingCoroutineStopToken.Interrupt();

            lineText.maxVisibleCharacters = 10000;

            if (skipSFX != null)
                App.Manager.Sound.PlaySFX(skipSFX);
        }
        else
        {
            if (!doesUserContinueRequest && skipSFX != null)
                App.Manager.Sound.PlaySFX(skipSFX);
            doesUserContinueRequest = true;
        }
    }

    public override void DialogueStarted()
    {
        for (int i = 0; i < skipButton.Length; i++) 
        {
            //skipButton[i].onClick.RemoveAllListeners();
            skipButton[i].onClick.AddListener(UserRequestedViewAdvancement);
        }
        
        //skipButton.gameObject.SetActive(true);
    }

    public override void DialogueComplete()
    {
        
    }

    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        //isRunningLine = true;

        StartCoroutine(UpdateLine(dialogueLine, onDialogueLineFinished));
    }

    private IEnumerator UpdateLine(LocalizedLine _localizedLine, Action _onDialogueLineFinished)
    {
        doesUserSkipRequest = false;
        doesUserContinueRequest = false;
        isStartLine = true;

        lineText.text = _localizedLine.Text.Text;

        typingCoroutineStopToken = new Effects.CoroutineInterruptToken();


        yield return StartCoroutine(Effects.Typewriter(lineText, 30f, null, typingCoroutineStopToken)); // Ÿ���� ����Ʈ

        isStartLine = false;
        doesUserSkipRequest = true; // ���� ��ŵ ��û �Ұ�(Ÿ������ ��� �Ϸ�Ǿ����Ƿ�)

        yield return new WaitUntil(() => doesUserContinueRequest); // ������ �������� ��ư Ŭ���� �� ���� ���

        _onDialogueLineFinished?.Invoke();

        //isRunningLine = false;
    }

    void Update()
    {
        if (textSFX == null) return;
        if(!doesUserSkipRequest && isStartLine)
        {
            if (!App.Manager.Sound.IsPlayingTypeWriteSFX())
                App.Manager.Sound.PlayTypeWriteSFX(textSFX);
        }
    }
}
