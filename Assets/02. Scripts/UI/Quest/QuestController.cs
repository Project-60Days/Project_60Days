using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestController : MonoBehaviour
{
    [SerializeField] GameObject questList;
    [SerializeField] GameObject mainQuestPrefab;
    [SerializeField] GameObject subQuestPrefab;
    [SerializeField] Transform questParent;

    public QuestBase[] quests;

    public List<QuestBase> currentQuest = new List<QuestBase>();
    public List<GameObject> currentQuestPrefab = new List<GameObject>();

    void Awake()
    {
        quests = questList.GetComponentsInChildren<QuestBase>();
    }

    public void CreateQuest(string _questCode)
    {
        foreach (var quest in quests)
            if (quest.questCode == _questCode && quest.isMeetOnce == false)
                AddQuest(quest);
    }

    void AddQuest(QuestBase _quest)
    {
        GameObject obj;

        if (_quest.eQuestType == EQuestType.Main)
            obj = Instantiate(mainQuestPrefab, questParent);
        else
            obj = Instantiate(subQuestPrefab, questParent);

        TMP_Text questText = obj.transform.GetComponentInChildren<TMP_Text>();
        questText.text = _quest.SetQuestText();

        currentQuest.Add(_quest);
        currentQuestPrefab.Add(obj);

        SetQuestList();
    }

    /// <summary>
    /// 퀘스트 리스트 순서 정렬 함수 (임시로 메인퀘스트는 위에, 서브퀘스트는 아래에 위치하게 설정)
    /// </summary>
    void SetQuestList()
    {
        QuestBase[] quests = questParent.GetComponentsInChildren<QuestBase>();

        foreach (QuestBase quest in quests)
        {
            if (quest.eQuestType == EQuestType.Main)
                quest.transform.SetAsFirstSibling();
            else
                quest.transform.SetAsLastSibling();
        }
    }

    public void CheckCurrentQuest()
    {
        List<QuestBase> tempList = new List<QuestBase>(currentQuest);

        foreach (var quest in tempList)
        {
            if (quest.CheckMeetCondition() == true) 
            {
                int index = currentQuest.IndexOf(quest);
                quest.isMeetOnce = true;
                Destroy(currentQuestPrefab[index]);
                currentQuest.RemoveAt(index);
                currentQuestPrefab.RemoveAt(index);
            }
        }
    }
}
