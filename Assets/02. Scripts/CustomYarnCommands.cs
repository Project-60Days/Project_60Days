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
        dialogueRunner.AddCommandHandler("mapNextDay", MapNextDay);
        dialogueRunner.AddCommandHandler("moveToLab", MoveToLab);
        dialogueRunner.AddCommandHandler("waitLightDown", WaitLightDown);
        dialogueRunner.AddCommandHandler("waitLightUp", WaitLightUp);
        dialogueRunner.AddCommandHandler("endTutorial", EndTutorial);
        dialogueRunner.AddCommandHandler<int>("moveNoteTap", MoveNoteTap);
        dialogueRunner.AddCommandHandler<string>("waitGetItem", WaitGetItem);
        dialogueRunner.AddCommandHandler<string, int>("waitSetCraftingItem", WaitSetCraftingItem);
        dialogueRunner.AddCommandHandler("waitNewDay", WaitNewDay);
        dialogueRunner.AddCommandHandler("waitTileUIOpen", WaitTileUIOpen);
        dialogueRunner.AddCommandHandler("waitTutorialTileUIOpen", WaitTutorialTileUiOpen);
        dialogueRunner.AddCommandHandler("waitSetDisturbance", WaitSetDisturbance);
        dialogueRunner.AddCommandHandler("spawnTutorialGlicher", SpawnTutorialGlicher);
        dialogueRunner.AddCommandHandler<string>("waitUntil", WaitUntilUIState);
        dialogueRunner.AddCommandHandler("hide", HideDialogue);
        dialogueRunner.AddCommandHandler("show", ShowDialogue);
        dialogueRunner.AddCommandHandler("nextTutorial", SetNextTutorial);
        dialogueRunner.AddCommandHandler<string, string>("highlight", HighLightObject);
        dialogueRunner.AddCommandHandler<string>("play_bgm", PlayBGM);
        dialogueRunner.AddCommandHandler<string>("play_sfx", PlaySFX);
        dialogueRunner.AddCommandHandler<string>("stop_bgm", StopBGM);
    }

    private void MoveToLab()
    {
        App.instance.GetMapManager().SetMapCameraPriority(false);
    }

    private void EndTutorial()
    {
        TutorialManager.instance.EndTutorial();
        UIManager.instance.GetEndUIController().Show();
    }

    private void HideDialogue()
    {
        UIManager.instance.GetTutorialDialogue().Hide();
    }
    private void ShowDialogue()
    {
        UIManager.instance.GetTutorialDialogue().Show();
    }

    private Coroutine WaitLightDown()
    {
        TutorialManager.instance.LightDownBackground();
        return StartCoroutine(new WaitUntil(() => !TutorialManager.instance.isLightUp));
    }

    private Coroutine WaitLightUp()
    {
        TutorialManager.instance.LightUpBackground();
        return StartCoroutine(new WaitUntil(() => TutorialManager.instance.isLightUp));
    }

    private Coroutine WaitTutorialTileUiOpen()
    {
        //MapController.instance.OffCurrentUI();
        //return StartCoroutine(new WaitUntil(() => MapController.instance.isTutorialUiOn()));

        // UI컨트롤러 만들고 추후 수정 필요
        App.instance.GetMapManager().OnTargetPointUI();
        return StartCoroutine(new WaitUntil(() => App.instance.GetMapManager().CheckCanInstallDrone()));
    }

    private Coroutine WaitGetItem(string _itemCode)
    {
        return StartCoroutine(new WaitUntil(() => UIManager.instance.GetInventoryController().CheckInventoryItem(_itemCode)));
    }

    private Coroutine WaitSetCraftingItem(string _itemCode, int _count = 1)
    {
        return StartCoroutine(new WaitUntil(() => UIManager.instance.GetCraftingUIController().CheckCraftingItem(_itemCode, _count)));
    }

    private Coroutine WaitNewDay()
    {
        return StartCoroutine(new WaitUntil(() => UIManager.instance.GetNoteController().GetNewDay()));
    }

    private void MoveNoteTap(int _idx)
    {
        Debug.Log("MovenoteTap" + _idx.ToString());

        if (_idx == 0) App.instance.GetMapManager().SetMapCameraPriority(false);
        else App.instance.GetMapManager().SetMapCameraPriority(true);
        UIManager.instance.GetNoteController().ChangePageForce(_idx);
    } 

    private void SpawnTutorialGlicher()
    {
        MapController.instance.SpawnTutorialZombie();
    }

    private void MapNextDay()
    {
        MapController.instance.NextDay();
        MapController.instance.NextDay();
    }

    private Coroutine WaitSetDisturbance()
    {
        // UI컨트롤러 만들고 추후 수정 필요
        //MapUiController.instance.InteractableOn();
        return StartCoroutine(new WaitUntil(() => MapController.instance.isDisturbtorInstall()));
    }

    private Coroutine WaitTileUIOpen()
    {
        //return StartCoroutine(new WaitUntil(() => MapController.instance.UIOn));
        // UI컨트롤러 만들고 추후 수정 필요
        App.instance.GetMapManager().OnTargetPointUI();
        return StartCoroutine(new WaitUntil(() => App.instance.GetMapManager().CheckCanInstallDrone()));
    }

    private Coroutine WaitUntilUIState(string _UIName)
    {
        return StartCoroutine(new WaitUntil(() => UIManager.instance.isUIStatus(_UIName))); 
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
        App.instance.GetSoundManager().PlayBGM(bgmName);
    }

    private void PlaySFX(string sfxName)
    {
        App.instance.GetSoundManager().PlaySFX(sfxName);
    }

    private void StopBGM(string soundName)
    {
        App.instance.GetSoundManager().StopBGM();
    }
}