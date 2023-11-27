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

    int nextTutorialIndex = 0;
    int nextMainIndex = 0;
    int nextSubIndex = 0;


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

    public void StartMainQuest()
    {
        CreateQuest("chapter01_GetNetworkChip");
    }

    /// <summary>
    /// 첫 번째 퀘스트 생성 함수
    /// </summary>
    /// <param name="_questCode"></param>
    public void CreateQuest(string _questCode)
    {
        foreach (var quest in quests)
            if (quest.questCode == _questCode)
                AddQuest(quest);
    }

    /// <summary>
    /// 퀘스트 생성 함수
    /// </summary>
    /// <param name="_quest"></param>
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
        PlayQuestAnim(obj);

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

    /// <summary>
    /// 퀘스트 생성 시 애니메이션 재생 함수
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
    public void SetNextQuestIndex(EQuestType _type, int _nextIndex)
    {
        if (_type == EQuestType.Tutorial)
            nextTutorialIndex = _nextIndex;
        else if (_type == EQuestType.Main)
            nextMainIndex = _nextIndex;
        else
            nextSubIndex = _nextIndex;
    }

    /// <summary>
    /// 선행 퀘스트 종료 시 다음 퀘스트 시작
    /// </summary>
    /// <param name="_quest"></param>
    public void StartNextQuest(QuestBase _quest)
    {
        int index = currentQuest.IndexOf(_quest);
        DestoryQuest(index);
        CreateNextQuest(_quest.eQuestType);
    }

    /// <summary>
    /// 존재해있던 퀘스트 오브젝트 삭제
    /// </summary>
    /// <param name="_index"></param>
    public void DestoryQuest(int _index)
    {
        Destroy(currentQuestPrefab[_index]);
        currentQuest.RemoveAt(_index);
        currentQuestPrefab.RemoveAt(_index);
    }

    /// <summary>
    /// 다음 퀘스트 생성
    /// </summary>
    /// <param name="_type"></param>
    void CreateNextQuest(EQuestType _type)
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
    /// 퀘스트 타입에 따른 List 반환
    /// </summary>
    /// <param name="_type"></param>
    /// <returns></returns>
    List<QuestBase> GetQuestList(EQuestType _type)
    {
        if (_type == EQuestType.Tutorial)
            return tutorialQuests;
        else if (_type == EQuestType.Main)
            return mainQuests;
        else
            return subQuests;
    }

    /// <summary>
    /// 퀘스트 타입에 따른 다음 퀘스트 인덱스 반환
    /// </summary>
    /// <param name="_type"></param>
    /// <returns></returns>
    int GetQuestIndex(EQuestType _type)
    {
        if (_type == EQuestType.Tutorial)
            return nextTutorialIndex;
        else if (_type == EQuestType.Main)
            return nextMainIndex;
        else
            return nextSubIndex;
    }
    #endregion
}
