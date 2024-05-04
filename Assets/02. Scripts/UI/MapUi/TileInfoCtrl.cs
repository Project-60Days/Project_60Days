using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum TileInfo
{
    Landform,
    Zombie,
    Resource
}

public class TileInfoCtrl : MonoBehaviour
{
    [SerializeField] Image illustration;
    [SerializeField] TextMeshProUGUI[] texts;

    public void UpdateImage(Sprite sprite)
    {
        illustration.sprite = sprite;
    }

    public void UpdateText(TileInfo infoTMP, string text)
    {
        texts[(int)infoTMP].text = text;
    }
}
