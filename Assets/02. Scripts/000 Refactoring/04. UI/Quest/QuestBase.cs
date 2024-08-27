using UnityEngine;
using TMPro;

public class QuestBase : MonoBehaviour
{
    public string questCode;
    public QuestType type;

    private void Start()
    {
        string code = "STR_QUEST_" + questCode;
        GetComponentInChildren<TextMeshProUGUI>().text = App.Data.Game.GetString(code);
    }
}
