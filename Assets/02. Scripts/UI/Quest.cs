using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Quest : MonoBehaviour
{
    EQuestType type;
    
    public void SetEQuestType(EQuestType type)
    {
        this.type = type;
    }

    public EQuestType GetEQuestType()
    {
        return this.type;
    }

    public void SetQuestTypeText()
    {
        TMP_Text questText = this.transform.GetChild(0).GetComponent<TMP_Text>();
        if (this.type == EQuestType.Main)
            questText.text = "주 목표: ";
        else 
            questText.text = "보조 목표: ";
    }
    
    public void SetQuestTypeImage()
    {
        Image image = this.GetComponent<Image>();
        if (this.type == EQuestType.Main)
            image.color=Color.red;
        else
            image.color = Color.blue;
    }

    public void SetText(string text)
    {
        TMP_Text questText = this.transform.GetChild(1).GetComponent<TMP_Text>();
        questText.text = text;
    }
}
