using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TitleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Text buttonText;
    Image buttonImage;

    Color normalTextColor = Color.white;
    Color highlightTextColor = Color.black;

    [SerializeField] GameObject titlePanel;
    [SerializeField] GameObject optionPanel;
    [SerializeField] GameObject soundPanel;





    void Start()
    {
        buttonText = GetComponentInChildren<Text>();
        buttonImage = GetComponentInChildren<Image>();

        optionPanel.SetActive(false);
        soundPanel.SetActive(false);

        SetButtonNomal();
    }





    void SetButtonNomal()
    {
        buttonText.color = normalTextColor;
        buttonImage.enabled = false;
    }

    void SetButtonHighlighted()
    {
        buttonText.color = highlightTextColor;
        buttonImage.enabled = true;
    }





    public void OnPointerEnter(PointerEventData eventData)
    {
        SetButtonHighlighted();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetButtonNomal();
    }





    /// <summary>
    /// 옵션버튼을 눌렀을 때
    /// </summary>
    public void OpenOptionPanel()
    {
        titlePanel.SetActive(false);
        optionPanel.SetActive(true);
    }

    /// <summary>
    /// 옵션 화면에서 타이틀로 돌아가기 버튼을 눌렀을 때
    /// </summary>
    public void OpenTitlePanel()
    {
        titlePanel.SetActive(true);
        optionPanel.SetActive(false);
    }





    /// <summary>
    /// 옵션 화면에서 사운드 조절 버튼을 눌렀을 때
    /// </summary>
    public void OpenSoundPanel()
    {
        soundPanel.SetActive(true);
    }
}