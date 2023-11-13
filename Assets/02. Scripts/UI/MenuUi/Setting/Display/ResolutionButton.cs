using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResolutionButton : ButtonBase
{
    TextMeshProUGUI currentResolution;
    TextMeshProUGUI buttonText;
    Image buttonImage;

    Color normalTextColor = Color.white;
    Color clickedTextColor = Color.black;

    Color normalImageColor = Color.black;
    Color clickedImageColor = Color.white;

    public int width { get; private set; }
    public int height { get; private set; }

    DisplayController displayController;

    void Awake()
    {
        currentResolution = GameObject.Find("CurrentResolution_Txt").GetComponent<TextMeshProUGUI>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        buttonImage = GetComponent<Image>();
        displayController = GameObject.Find("Display_Back").GetComponent<DisplayController>();
        GetComponent<Button>().onClick.AddListener(ClickEvent);
        SetWidthNHeight();
    }

    void SetWidthNHeight()
    {
        int[] numbers = ExtractNumbersFromText();
        width = numbers[0];
        height = numbers[1];
    }

    int[] ExtractNumbersFromText()
    {
        string[] parts = buttonText.text.Split('x');

        for (int i = 0; i < parts.Length; i++)
            parts[i] = parts[i].Trim();

        int[] numbers = new int[parts.Length];

        for (int i = 0; i < parts.Length; i++)
            if (int.TryParse(parts[i], out int number))
                numbers[i] = number;

        return numbers;
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

    void ClickEvent()
    {
        currentResolution.text = buttonText.text;
        displayController.SetResolution(this);
    }
}
