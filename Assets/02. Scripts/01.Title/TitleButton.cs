using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class TitleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Text buttonText;
    private Image buttonImage;

    private Color normalTextColor = Color.white;
    private Color highlightTextColor = Color.black;

    [SerializeField] int flag;

    [SerializeField] GameObject titlePanel;
    [SerializeField] GameObject optionPanel;
    [SerializeField] GameObject soundPanel;

    private void Start()
    {
        buttonText = GetComponentInChildren<Text>();
        buttonImage = GetComponentInChildren<Image>();

        buttonText.color = normalTextColor;
        buttonImage.enabled = false;
    }

    /// <summary>
    /// Highlighted 상태일 때
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.color = highlightTextColor;
        buttonImage.enabled = true;
    }

    /// <summary>
    /// Normal 상태로 돌아갔을 때
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.color = normalTextColor;
        buttonImage.enabled = false;
    }

    public void OpenOptionPanel()
    {
        titlePanel.SetActive(false);
        optionPanel.SetActive(true);
    }

    public void OpenTitlePanel()
    {
        titlePanel.SetActive(true);
        optionPanel.SetActive(false);
    }

    public void OpenSoundPanel()
    {
        soundPanel.SetActive(true);
    }
}