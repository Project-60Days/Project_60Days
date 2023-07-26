using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using DG.Tweening;
using Yarn;
using UnityEngine.UIElements;
using System;

public class CustomYarnCommands : Singleton<CustomYarnCommands>
{
    [SerializeField] DialogueRunner dialogueRunner;
    [SerializeField] CustomDialogueView dialogueView;

    void Awake()
    {
        dialogueRunner.AddCommandHandler("nextTutorial", SetNextTutorial);
        dialogueRunner.AddCommandHandler<string, string>("highlight", HighLightObject);
        dialogueRunner.AddCommandHandler<string>("play_bgm", PlayBGM);
        dialogueRunner.AddCommandHandler<string>("play_sfx", PlaySFX);
        dialogueRunner.AddCommandHandler<string>("stop_bgm", StopBGM);
    }

    private void SetNextTutorial()
    {
        TutorialManager.instance.tutorialController.SetNextTutorial();
    }

    private void HighLightObject(string _objectID, string _waitStatusName)
    {
        UIManager.instance.GetUIHighLightController().ShowHighLight(_objectID, _waitStatusName);
    }

    private void PlayBGM(string bgmName)
    {
        SoundManager.instance.PlayBGM(bgmName);
    }

    private void PlaySFX(string sfxName)
    {
        SoundManager.instance.PlaySFX(sfxName);
    }

    private void StopBGM(string soundName)
    {
        SoundManager.instance.StopBGM();
    }
}