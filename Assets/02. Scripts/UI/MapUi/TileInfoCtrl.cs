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
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI[] texts;

    public void SetImage(Sprite sprite)
    {
        image.sprite = sprite;
    }

    public void SetText(TileInfo infoTMP, string text)
    {
        texts[(int)infoTMP].text = text;
    }
}
