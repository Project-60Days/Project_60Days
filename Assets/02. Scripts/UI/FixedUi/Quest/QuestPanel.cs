using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class QuestPanel : UIBase
{
    [SerializeField] GameObject questList;
    [SerializeField] GameObject mainQuestPrefab;
    [SerializeField] GameObject subQuestPrefab;
    [SerializeField] Transform questParent;

    QuestBase[] quests;
    List<QuestBase> tutorialQuests = new List<QuestBase>();
    List<QuestBase> mainQuests = new List<QuestBase>();
    List<QuestBase> subQuests = new List<QuestBase>();

    List<QuestBase> currentQuest = new List<QuestBase>();
    List<GameObject> currentQuestPrefab = new List<GameObject>();

    int nextTutorialIndex = 0;
    int nextMainIndex = 0;
    int nextSubIndex = 0;

    #region Override
    public override void Init()
    {
        quests = questList.GetComponentsInChildren<QuestBase>();
        foreach (var quest in quests)
        {
            if (quest.type == QuestType.Tutorial)
                tutorialQuests.Add(quest);
            else if (quest.type == QuestType.Main)
                mainQuests.Add(quest);
            else
                subQuests.Add(quest);
        }
    }

    public override void ReInit() { }
    #endregion

    public void StartMainQuest()
    {
        CreateQuest("chapter01_AccessProductionStructure");
    }

    /// <summary>
    /// ù ��° ����Ʈ ���� �Լ�
    /// </summary>
    /// <param name="_questCode"></param>
    public void CreateQuest(string _questCode)
    {
        foreach (var quest in quests)
            if (quest.questCode == _questCode)
                AddQuest(quest);
    }

    /// <summary>
    /// ����Ʈ ���� �Լ�
    /// </summary>
    /// <param name="_quest"></param>
    void AddQuest(QuestBase _quest)
    {
        GameObject obj;

        if (_quest.type == QuestType.Sub)
            obj = Instantiate(subQuestPrefab, questParent);
        else
            obj = Instantiate(mainQuestPrefab, questParent);

        TMP_Text questText = obj.transform.GetComponentInChildren<TMP_Text>();
        questText.text = _quest.SetQuestText();

        currentQuest.Add(_quest);
        currentQuestPrefab.Add(obj);

        SetQuestList();
        PlayQuestAnim(obj);

        StartCoroutine(_quest.CheckQuestComplete());
    }


    /// <summary>
    /// ����Ʈ ����Ʈ ���� ���� �Լ� (�ӽ÷� ��������Ʈ�� ����, ��������Ʈ�� �Ʒ��� ��ġ�ϰ� ����)
    /// </summary>
    void SetQuestList()
    {
        if (currentQuest.Count >= 2)
        {
            for (int i = 0; i < currentQuestPrefab.Count; i++)
            {
                RectTransform questPrefab = currentQuestPrefab[i].GetComponent<RectTransform>();

                if (currentQuest[i].type == QuestType.Main)
                    questPrefab.localPosition = new Vector3(questPrefab.localPosition.x, 0, 0);
                else
                {
                    float yPos = -i * (questPrefab.rect.height);
                    questPrefab.localPosition = new Vector3(questPrefab.localPosition.x, yPos, 0);
                }
            }
        }
        else if (currentQuest.Count == 1)
        {
            RectTransform questPrefab = currentQuestPrefab[0].GetComponent<RectTransform>();
            questPrefab.localPosition = new Vector3(questPrefab.localPosition.x, 0, 0);
        }
        else return;
    }

    /// <summary>
    /// ����Ʈ ���� �� �ִϸ��̼� ��� �Լ�
    /// </summary>
    /// <param name="_obj"></param>
    void PlayQuestAnim(GameObject _obj)
    {
        RectTransform objRect = _obj.GetComponent<RectTransform>();
        float width = objRect.rect.width;
        _obj.transform.DOLocalMoveX(width, 0.3f)
            .OnComplete(() => objRect.localPosition = new Vector3(width, objRect.localPosition.y, 0));
    }





    #region next quest
    public void SetNextQuestIndex(QuestType _type, int _nextIndex)
    {
        if (_type == QuestType.Tutorial)
            nextTutorialIndex = _nextIndex;
        else if (_type == QuestType.Main)
            nextMainIndex = _nextIndex;
        else
            nextSubIndex = _nextIndex;
    }

    /// <summary>
    /// ���� ����Ʈ ���� �� ���� ����Ʈ ����
    /// </summary>
    /// <param name="_quest"></param>
    public void StartNextQuest(QuestBase _quest)
    {
        int index = currentQuest.IndexOf(_quest);
        currentQuestPrefab[index].GetComponent<CanvasGroup>().DOFade(0.0f, 0.3f).SetLoops(5, LoopType.Yoyo).OnComplete(() =>
        {
            DestoryQuest(index);
            CreateNextQuest(_quest.type);
        });
    }

    /// <summary>
    /// �������ִ� ����Ʈ ������Ʈ ����
    /// </summary>
    /// <param name="_index"></param>
    public void DestoryQuest(int _index)
    {
        Destroy(currentQuestPrefab[_index]);
        currentQuest.RemoveAt(_index);
        currentQuestPrefab.RemoveAt(_index);
    }

    /// <summary>
    /// ���� ����Ʈ ����
    /// </summary>
    /// <param name="_type"></param>
    void CreateNextQuest(QuestType _type)
    {
        var quests = GetQuestList(_type);
        var nextQuestIndex = GetQuestIndex(_type);

        if (nextQuestIndex == -1)
        {
            SetQuestList();
            return;
        }

        foreach (var quest in quests)
            if (quest.questIndex == nextQuestIndex)
                AddQuest(quest);
    }

    /// <summary>
    /// ����Ʈ Ÿ�Կ� ���� List ��ȯ
    /// </summary>
    /// <param name="_type"></param>
    /// <returns></returns>
    List<QuestBase> GetQuestList(QuestType _type)
    {
        if (_type == QuestType.Tutorial)
            return tutorialQuests;
        else if (_type == QuestType.Main)
            return mainQuests;
        else
            return subQuests;
    }

    /// <summary>
    /// ����Ʈ Ÿ�Կ� ���� ���� ����Ʈ �ε��� ��ȯ
    /// </summary>
    /// <param name="_type"></param>
    /// <returns></returns>
    int GetQuestIndex(QuestType _type)
    {
        if (_type == QuestType.Tutorial)
            return nextTutorialIndex;
        else if (_type == QuestType.Main)
            return nextMainIndex;
        else
            return nextSubIndex;
    }
    #endregion
}
