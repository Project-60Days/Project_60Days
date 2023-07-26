using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using TMPro;
using System;

public class CustomDialogueView : DialogueViewBase
{
    [Header("0725 임시 개발")]
    [SerializeField] TutorialDialogue tutorialDialogue;


    [SerializeField] private Button skipButton;
    [SerializeField] private TextMeshProUGUI lineText;

    private bool isRunningLine = false;
    private bool doesUserContinueRequest = false;
    private bool doesUserSkipRequest = false;

    Effects.CoroutineInterruptToken typingCoroutineStopToken;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {

    }

    // 유저가 스킵 버튼 눌렀을 때 호출되는 함수
    public override void UserRequestedViewAdvancement()
    {
        if (!doesUserSkipRequest)
        {
            doesUserSkipRequest = true;

            if (typingCoroutineStopToken.CanInterrupt)
                typingCoroutineStopToken.Interrupt();

            lineText.maxVisibleCharacters = 10000;
        }
        else
        {
            doesUserContinueRequest = true;
        }
    }

    public override void DialogueStarted()
    {
        skipButton.onClick.RemoveAllListeners();
        skipButton.onClick.AddListener(UserRequestedViewAdvancement);

        skipButton.gameObject.SetActive(true);
    }

    public override void DialogueComplete()
    {
        tutorialDialogue.EndDialogue();
    }

    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        isRunningLine = true;

        StartCoroutine(UpdateLine(dialogueLine, onDialogueLineFinished));
    }

    private IEnumerator UpdateLine(LocalizedLine _localizedLine, Action _onDialogueLineFinished)
    {
        //if (_localizedLine.Metadata?.Length > 0)
        //{
        //    foreach (var metaData in _localizedLine.Metadata)
        //    {
        //        Debug.LogError($"Tag Detected : {metaData}");
        //        ReadYarnTag(metaData, _localizedLine.CharacterName);
        //    }
        //}

        skipButton.gameObject.SetActive(true);

        doesUserSkipRequest = false;
        doesUserContinueRequest = false;

        lineText.text = _localizedLine.Text.Text;

        typingCoroutineStopToken = new Effects.CoroutineInterruptToken();


        yield return StartCoroutine(Effects.Typewriter(lineText, 30f, null, typingCoroutineStopToken)); // 타이핑 이펙트


        doesUserSkipRequest = true; // 유저 스킵 요청 불가(타이핑이 모두 완료되었으므로)

        yield return new WaitUntil(() => doesUserContinueRequest); // 유저가 다음으로 버튼 클릭할 때 까지 대기


        _onDialogueLineFinished?.Invoke();

        isRunningLine = false;
    }
}
