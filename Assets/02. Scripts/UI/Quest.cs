using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Quest : MonoBehaviour
{
    EQuestType type;
    [SerializeField] Sprite[] questImages;
    [SerializeField] Sprite[] progressImages;
    
    public void SetEQuestType(EQuestType _type)
    {
        this.type = _type;
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
        Image[] images = transform.GetComponentsInChildren<Image>();
        if (type == EQuestType.Main)
        {
            images[0].sprite = questImages[0];
            images[1].sprite = progressImages[0];
        }
        else
        {
            images[0].sprite = questImages[1];
            images[1].sprite = progressImages[1];
        }
    }
}
