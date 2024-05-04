using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "Quest", menuName = "Scriptable Object/Quest")]
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
