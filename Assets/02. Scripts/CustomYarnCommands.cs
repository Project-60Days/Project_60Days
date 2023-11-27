using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using DG.Tweening;
using Yarn;
using UnityEngine.UIElements;
using System;
using System.Xml.Serialization;

public class CustomYarnCommands : Singleton<CustomYarnCommands>
{
    [SerializeField] DialogueRunner dialogueRunner;

    void Awake()
    {
        //common//
        dialogueRunner.AddCommandHandler<string, string>("highlight", HighLightObject);
        dialogueRunner.AddCommandHandler<string>("waitUntil", WaitUntilUIState);
        dialogueRunner.AddCommandHandler("hide", HideDialogue);
        dialogueRunner.AddCommandHandler("show", ShowDialogue);
        dialogueRunner.AddCommandHandler<string>("setQuest", SetQuest);

        //01//
        dialogueRunner.AddCommandHandler("lightUpWorkBench", LightUpWorkBench);
        dialogueRunner.AddCommandHandler("lightDownWorkBench", LightDownWorkBench);

        //02//
        dialogueRunner.AddCommandHandler<string>("waitGetItem", WaitGetItem);

        //03//
        dialogueRunner.AddCommandHandler("waitLightUp", WaitLightUp);
        dialogueRunner.AddCommandHandler<string, bool>("setAlert", SetAlertState);
        dialogueRunner.AddCommandHandler("closeNote", CloseNote);

        //05//
        dialogueRunner.AddCommandHandler("waitMovePoint", WaitMovePoint);

        //06//
        dialogueRunner.AddCommandHandler("waitNewDay", WaitNewDay);

        //08//
        dialogueRunner.AddCommandHandler("endTutorial", EndTutorial);

        dialogueRunner.AddCommandHandler("appendNode", AppendNode);

        //dialogueRunner.AddCommandHandler("spawnTutorialGlicher", SpawnTutorialGlicher);
        //dialogueRunner.AddCommandHandler<string>("play_bgm", PlayBGM);
        //dialogueRunner.AddCommandHandler<string>("play_sfx", PlaySFX);
        //dialogueRunner.AddCommandHandler<string>("stop_bgm", StopBGM);
    }





    void AppendNode()
    {
        string nodeName = UIManager.instance.GetPageController().GetNextResourceNodeName();

        Debug.Log(nodeName);
        if (nodeName == "-1") return;

        UIManager.instance.GetPageController().CreateDialogueRunner(nodeName);
    }





    #region common
    void HighLightObject(string _objectID, string _waitStatusName)
    {
        UIManager.instance.GetUIHighLightController().ShowHighLight(_objectID, _waitStatusName);
    }

    Coroutine WaitUntilUIState(string _UIName)
    {
        return StartCoroutine(new WaitUntil(() => UIManager.instance.isUIStatus(_UIName)));
    }

    void HideDialogue()
    {
        TutorialManager.instance.GetTutorialController().Hide();
    }

    void ShowDialogue()
    {
        TutorialManager.instance.GetTutorialController().Show();
    }

    void SetQuest(string _questCode)
    {
        UIManager.instance.GetQuestController().CreateQuest(_questCode);
    }
    #endregion





    #region 01
    void LightUpWorkBench()
    {
        TutorialManager.instance.GetTutorialController().LightUpWorkBench();
    }

    void LightDownWorkBench()
    {
        TutorialManager.instance.GetTutorialController().LightDownWorkBench();
    }
    #endregion






    #region 02
    private Coroutine WaitGetItem(string _itemCode)
    {
        return StartCoroutine(new WaitUntil(() => UIManager.instance.GetInventoryController().CheckInventoryItem(_itemCode)));
    }
    #endregion





    #region 03
    private Coroutine WaitLightUp()
    {
        return StartCoroutine(new WaitUntil(() => TutorialManager.instance.GetTutorialController().isLightUp));
    }

    void SetAlertState(string _alertType, bool _isActive)
    {
        UIManager.instance.GetAlertController().SetAlert(_alertType, _isActive);
    }

    void CloseNote()
    {
        UIManager.instance.GetNoteController().CloseNote();
    }
    #endregion





    #region 05
    Coroutine WaitMovePoint()
    {
        return StartCoroutine(new WaitUntil(() => App.instance.GetMapManager().mapUIController.MovePointActivate()));
    }
    #endregion





    #region 06
    Coroutine WaitNewDay()
    {
        return StartCoroutine(new WaitUntil(() => UIManager.instance.GetNoteController().GetNewDay()));
    }
    #endregion





    #region 08
    void EndTutorial()
    {
        TutorialManager.instance.EndTutorial();
    }
    #endregion





    #region temp
    private void SpawnTutorialGlicher()
    {
        MapController.instance.SpawnTutorialZombie();
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
    #endregion


    [YarnFunction("getResourceCount")]
    public static int GetResourceCount(string _itemCode)
    {
        var resources = App.instance.GetMapManager().resourceManager.GetLastResources();

        int count = 0;

        for (int i = 0; i < resources.Count; i++)
        {
            if (resources[i].ItemBase.data.Code == _itemCode)
                count = resources[i].ItemCount;
        }

        return count;
    }
}