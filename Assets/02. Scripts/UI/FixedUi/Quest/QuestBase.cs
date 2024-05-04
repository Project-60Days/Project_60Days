using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Scriptable Object/Quest")]
public abstract class QuestBase : MonoBehaviour
{
    public string questCode;
    public QuestType type;
    public string stringCode;
}
