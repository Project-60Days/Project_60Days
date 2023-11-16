using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class QuestController : MonoBehaviour
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

    [HideInInspector] public int nextTutorialIndex = 0;
    [HideInInspector] public int nextMainIndex = 0;
    [HideInInspector] public int nextSubIndex = 0;

    void Awake()
    {
        quests = questList.GetComponentsInChildren<QuestBase>();
        foreach (var quest in quests) 
        {
            if (quest.eQuestType == EQuestType.Tutorial)
                tutorialQuests.Add(quest);
            else if (quest.eQuestType == EQuestType.Main)
                mainQuests.Add(quest);
            else
                subQuests.Add(quest);

        }
    }

    /// <summary>
    /// 테스트용 퀘스트 생성 함수
    /// </summary>
    /// <param name="_questCode"></param>
    public void CreateQuest(string _questCode)
    {
        foreach (var quest in quests)
            if (quest.questCode == _questCode)
                AddQuest(quest);
    }

    void AddQuest(QuestBase _quest)
    {
        GameObject obj;

        if (_quest.eQuestType == EQuestType.Sub)
            obj = Instantiate(subQuestPrefab, questParent);
        else
            obj = Instantiate(mainQuestPrefab, questParent);

        TMP_Text questText = obj.transform.GetComponentInChildren<TMP_Text>();
        questText.text = _quest.SetQuestText();

        currentQuest.Add(_quest);
        currentQuestPrefab.Add(obj);

        SetQuestList();
        AppearQuest(obj);

        StartCoroutine(_quest.CheckQuestComplete());
    }

    /// <summary>
    /// 퀘스트 리스트 순서 정렬 함수 (임시로 메인퀘스트는 위에, 서브퀘스트는 아래에 위치하게 설정)
    /// </summary>
    void SetQuestList()
    {
        if (currentQuest.Count >= 2)
        {
            for (int i = 0; i < currentQuestPrefab.Count; i++)
            {
                RectTransform questPrefab = currentQuestPrefab[i].GetComponent<RectTransform>();

                if (currentQuest[i].eQuestType == EQuestType.Main)
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

    void AppearQuest(GameObject _obj)
    {
        RectTransform objRect = _obj.GetComponent<RectTransform>();
        float width = objRect.rect.width;
        _obj.transform.DOLocalMoveX(width, 0.3f)
            .OnComplete(() => objRect.localPosition = new Vector3(width, objRect.localPosition.y, 0));
    }

    public void StartNextQuest(QuestBase _quest)
    {
        int index = currentQuest.IndexOf(_quest);
        Destroy(currentQuestPrefab[index]);
        currentQuest.RemoveAt(index);
        currentQuestPrefab.RemoveAt(index);
        CreateNextQuest(_quest.eQuestType);
    }

    void CreateNextQuest(EQuestType _type)
    {
        var quests = GetQuestList(_type);
        var nextQuestIndex = GetQuestIndex(_type);
        foreach (var quest in quests)
        {
            if (quest.questIndex == -1)
            {
                SetQuestList();
                return;
            }
            if (quest.questIndex == nextQuestIndex)
                AddQuest(quest);
        }
    }

    List<QuestBase> GetQuestList(EQuestType _type)
    {
        if (_type == EQuestType.Tutorial)
            return tutorialQuests;
        else if (_type == EQuestType.Main)
            return mainQuests;
        else
            return subQuests;
    }

    int GetQuestIndex(EQuestType _type)
    {
        if (_type == EQuestType.Tutorial)
            return nextTutorialIndex;
        else if (_type == EQuestType.Main)
            return nextMainIndex;
        else
            return nextSubIndex;
    }
}
