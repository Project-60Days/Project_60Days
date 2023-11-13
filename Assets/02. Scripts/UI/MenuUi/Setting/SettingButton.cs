using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingButton : MonoBehaviour
{
    TextMeshProUGUI buttonText;
    Image buttonImage;

    Color normalTextColor = Color.white;
    Color clickedTextColor = Color.black;

    Color normalImageColor = Color.black;
    Color clickedImageColor = Color.white;

    void Awake()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        buttonImage = GetComponent<Image>();
    }

    public void SetButtonNormal()
    {
        buttonText.color = normalTextColor;
        buttonImage.color = normalImageColor;
    }

    public void SetButtonClicked()
    {
        buttonText.color = clickedTextColor;
        buttonImage.color = clickedImageColor;
    }
}
