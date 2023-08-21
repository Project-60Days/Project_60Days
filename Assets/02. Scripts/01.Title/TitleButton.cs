using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TitleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Text buttonText;
    private Image buttonImage;

    private Color normalTextColor = Color.white;
    private Color highlightTextColor = Color.black;

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