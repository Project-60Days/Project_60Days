using System.Collections;
using UnityEngine;
using Yarn.Unity;

public class CustomYarnCommands : MonoBehaviour
{
    [SerializeField] DialogueRunner dialogueRunner;

    void Awake()
    {
        //tutorial//
        dialogueRunner.AddCommandHandler<string, string>("highlight", HighLightObject);
        dialogueRunner.AddCommandHandler<string>("highlightBtn", HighLightBtn);
        dialogueRunner.AddCommandHandler<string>("waitUntil", WaitUntilUIState);
        dialogueRunner.AddCommandHandler("hide", HideDialogue);
        dialogueRunner.AddCommandHandler("show", ShowDialogue);
        dialogueRunner.AddCommandHandler<string>("setQuest", SetQuest);
        dialogueRunner.AddCommandHandler<bool>("setCloseBtnEnabled", SetCloseBtnEnabled);
        dialogueRunner.AddCommandHandler<int>("lightUpAndFillBattery", LightUpAndFillBattery);

        //01//
        dialogueRunner.AddCommandHandler("lightUpWorkBench", LightUpWorkBench);
        dialogueRunner.AddCommandHandler("lightDownWorkBench", LightDownWorkBench);

        //02//
        dialogueRunner.AddCommandHandler<string>("waitGetItem", WaitGetItem);

        //03//
        dialogueRunner.AddCommandHandler<string, bool>("setAlert", SetAlertState);
        dialogueRunner.AddCommandHandler("waitMoveScroll", WaitMoveScroll);
        dialogueRunner.AddCommandHandler<bool>("setScrollBar", SetScrollBar);

        //04//
        dialogueRunner.AddCommandHandler("lightUpMap", LightUpMap);
        dialogueRunner.AddCommandHandler("lightDownMap", LightDownMap);

        //05//
        dialogueRunner.AddCommandHandler("waitMovePoint", WaitMovePoint);
        dialogueRunner.AddCommandHandler("addResource", AddResource);

        //06//
        dialogueRunner.AddCommandHandler("waitNewDay", WaitNewDay);
        dialogueRunner.AddCommandHandler("enableBtn", EnableBtn);

        //08//
        dialogueRunner.AddCommandHandler("startPV", StartPV);
        dialogueRunner.AddCommandHandler("waitPVEnd", WaitPVEnd);
        dialogueRunner.AddCommandHandler("lightUp", LightUp);

        //09//
        dialogueRunner.AddCommandHandler("endTutorial", EndTutorial);

        //common//
        dialogueRunner.AddCommandHandler("appendNode", AppendNode);

        //dialogueRunner.AddCommandHandler("spawnTutorialGlicher", SpawnTutorialGlicher);
        //dialogueRunner.AddCommandHandler<string>("play_bgm", PlayBGM);
        //dialogueRunner.AddCommandHandler<string>("play_sfx", PlaySFX);
        //dialogueRunner.AddCommandHandler<string>("stop_bgm", StopBGM);
    }

    void AppendNode()
    {
        string nodeName = App.Manager.UI.GetPanel<PagePanel>().GetNextResourceNodeName();

        if (nodeName == "-1") return;

        App.Manager.UI.GetPanel<PagePanel>().CreateResultDialogueRunner(nodeName);
    }

    #region Tutorial Common Commands
    void HighLightObject(string _objectID, string _waitStatusName)
    {
        App.Manager.UI.GetPanel<HighLightPanel>().ShowHighLight(_objectID, _waitStatusName);
    }

    void HighLightBtn(string _objectID)
    {
        App.Manager.UI.GetPanel<HighLightPanel>().ShowBtnHighLight(_objectID);
    }

    Coroutine WaitUntilUIState(string _UIName)
    {
        UIState state = App.Manager.UI.StringToState(_UIName);
        return StartCoroutine(new WaitUntil(() => App.Manager.UI.CurrState == state));
    }

    void HideDialogue()
    {
        App.Manager.Tutorial.Ctrl.Hide();
    }

    void ShowDialogue()
    {
        App.Manager.Tutorial.Ctrl.Show();
    }

    void SetQuest(string _questCode)
    {
        App.Manager.UI.GetPanel<QuestPanel>().CreateQuest(_questCode);
    }

    void SetCloseBtnEnabled(bool _isEnabled)
    {
        App.Manager.UI.GetPanel<NotePanel>().SetCloseBtnEnabled(_isEnabled);
    }

    void LightUpAndFillBattery(int _num)
    {
        App.Manager.Shelter.LightUpAndFillBattery(_num);
    }
    #endregion

    #region Tutorial 01
    void LightUpWorkBench()
    {
        App.Manager.Shelter.LightUpWorkBench();
    }

    void LightDownWorkBench()
    {
        App.Manager.Shelter.LightDownWorkBench();
    }
    #endregion

    #region Tutorial 02
    Coroutine WaitGetItem(string _itemCode)
    {
        return StartCoroutine(new WaitUntil(() => App.Manager.UI.GetPanel<InventoryPanel>().CheckInventoryItem(_itemCode)));
    }
    #endregion

    #region Tutorial 03
    void SetAlertState(string _alertType, bool _isActive)
    {
        App.Manager.UI.GetPanel<FixedPanel>().SetAlert(_alertType, _isActive);
    }

    Coroutine WaitMoveScroll()
    {
        return StartCoroutine(new WaitUntil(() => App.Manager.UI.GetPanel<NotePanel>().CheckIfScrolledToEnd()));
    }

    void SetScrollBar(bool _isInteractable)
    {
        App.Manager.UI.GetPanel<NotePanel>().SetScrollBarInteractable(_isInteractable);
    }
    #endregion

    #region Tutorial 04
    void LightUpMap()
    {
        App.Manager.Shelter.LightUpMap();
    }

    void LightDownMap()
    {
        App.Manager.Shelter.LightDownMap();
    }
    #endregion

    #region Tutorial 05
    Coroutine WaitMovePoint()
    {
        return StartCoroutine(new WaitUntil(() => App.Manager.UI.GetPanel<MapPanel>().MovePointActivate()));
    }

    void AddResource()
    {
        App.Manager.Tutorial.Ctrl.AddSteel();
    }
    #endregion

    #region Tutorial 06
    Coroutine WaitNewDay()
    {
        return StartCoroutine(new WaitUntil(() => App.Manager.Game.isNewDay));
    }

    void EnableBtn()
    {
        App.Manager.Tutorial.Ctrl.EnableBtn();
    }
    #endregion

    #region Tutorial 08
    void StartPV()
    {
        App.Manager.UI.GetPanel<PVPanel>().Start01();
    }

    Coroutine WaitPVEnd()
    {
        return StartCoroutine(new WaitUntil(() => App.Manager.UI.GetPanel<PVPanel>().isEnd));
    }


    void LightUp()
    {
        App.Manager.Shelter.LightUpBackground();
    }
    #endregion

    #region Tutorial 09
    void EndTutorial()
    {
        App.Manager.Tutorial.EndTutorial();
    }
    #endregion

    [YarnFunction("getResourceName")]
    static string GetResourceName()
    {
        string resourceName = App.Manager.UI.GetPanel<PagePanel>().currResource;
        return resourceName;
    }

    [YarnFunction("getResourceIndex")]
    static int GetResourceIndex()
    {
        int resourceIndex = App.Manager.UI.GetPanel<PagePanel>().currResourceIndex;
        return resourceIndex;
    }


    [YarnFunction("getResourceCount")]
    static int GetResourceCount(string _itemCode)
    {
        var resources = App.Manager.Map.resourceCtrl.GetLastResources();

        int count = 0;

        for (int i = 0; i < resources.Count; i++)
        {
            if (resources[i].Item.Code == _itemCode)
                count = resources[i].Count;
        }

        return count;
    }


    [YarnCommand("custom_wait")]
    static IEnumerator CustomWait(float _time)
    {
        yield return new WaitForSeconds(_time);
    }


    [YarnFunction("getStructName")]
    static string GetStructName()
    {
        string structName = App.Manager.UI.GetPanel<PagePanel>().currStruct;
        
        return structName;
    }
}