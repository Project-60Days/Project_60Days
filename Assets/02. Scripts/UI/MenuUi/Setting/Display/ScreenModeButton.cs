using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScreenModeButton : ButtonBase
{
    [SerializeField] TextMeshProUGUI buttonText;
    [SerializeField] Image buttonImage;

    Color normalTextColor = Color.white;
    Color clickedTextColor = Color.black;

    Color normalImageColor = Color.black;
    Color clickedImageColor = Color.white;

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
