using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class NoteAnim : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Button closeBtn;
    [SerializeField] Button openBtn;
    [SerializeField] Button nextDayBtn;

    [SerializeField] GameObject boxTop;
    [SerializeField] GameObject boxBottom;
    [SerializeField] GameObject notePanel;
    
    [SerializeField] Image blackPanel;
    [SerializeField] Text dayText;
    [SerializeField] GameObject nextPage;
    [SerializeField] GameObject prevPage;
    [SerializeField] GameObject nextDay;

    [SerializeField] Sprite[] btnImages;

    Vector2 originalPos;
    Vector2 topOriginalPos;
    Vector2 bottomOriginalPos;

    bool isOpen = false;
    int dayCount = 1;

    NoteController noteController;

    void Start()
    {
        noteController = GameObject.Find("NoteController").GetComponent<NoteController>();

        topOriginalPos = boxTop.transform.position;
        bottomOriginalPos = boxBottom.transform.position;
        originalPos = transform.position;

        notePanel.GetComponent<Image>().DOFade(0f, 0f);
        blackPanel.DOFade(0f, 0f);
        nextPage.SetActive(false);
        prevPage.SetActive(false);
        nextDay.SetActive(false);
        blackPanel.gameObject.SetActive(false);
        dayText.gameObject.SetActive(false);

        openBtn.onClick.AddListener(Open_Anim);
        closeBtn.onClick.AddListener(Close_Anim);
        nextDayBtn.onClick.AddListener(NextDayEvent);
    }


    /// <summary>
    /// 상자 마우스 호버
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isOpen)
            transform.DOMoveY(originalPos.y + 100f, 0.5f);
    }
    /// <summary>
    /// 상자 마우스 호버 해제
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOMoveY(originalPos.y, 0.5f);
    }


    /// <summary>
    /// 상자 열림 애니메이션
    /// </summary>
    void Open_Anim()
    {
        if (!isOpen)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(boxTop.transform.DOMoveY(topOriginalPos.y + 900f, 0.5f))
                .Join(boxBottom.transform.DOMoveY(bottomOriginalPos.y + 150f, 0.5f))
                .Join(transform.DOMoveY(originalPos.y, 0.5f))
                .Append(notePanel.GetComponent<Image>().DOFade(1f, 0.5f))
                .OnComplete(() => OpenBox());

            DOTween.Kill(gameObject);
            closeBtn.DOKill();
            openBtn.DOKill();

            isOpen = true;
            openBtn.image.sprite = btnImages[0];
            closeBtn.image.sprite = btnImages[0];
            sequence.Play();
        }
    }
    /// <summary>
    /// 상자 닫힘 애니메이션
    /// </summary>
    void Close_Anim()
    {
        if (isOpen)
        {
            noteController.CloseBox();
            nextPage.SetActive(false);
            prevPage.SetActive(false);
            nextDay.SetActive(false);
            dayText.gameObject.SetActive(false);

            Sequence sequence = DOTween.Sequence();
            sequence.Append(notePanel.GetComponent<Image>().DOFade(0f, 0.5f))
                .Append(boxTop.transform.DOMoveY(topOriginalPos.y, 0.5f))
                .Join(boxBottom.transform.DOMoveY(bottomOriginalPos.y, 0.5f))
                .OnComplete(() => CloseBox());

            DOTween.Kill(gameObject);
            closeBtn.DOKill();
            openBtn.DOKill();

            isOpen = false;
            openBtn.image.sprite = btnImages[0];
            closeBtn.image.sprite = btnImages[0];
            sequence.Play();
        }
    }


    /// <summary>
    /// 다음 날로 넘어가는 애니메이션(페이드아웃)
    /// </summary>
    void NextDayEvent()
    {
        Close_Anim();
        blackPanel.gameObject.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(blackPanel.DOFade(1f, 1f)).SetEase(Ease.InQuint)
            .AppendInterval(0.5f)
            .Append(blackPanel.DOFade(0f, 1f))
            .OnComplete(() => NewDay());
        sequence.Play();
    }


    /// <summary>
    /// 상자 열림 콜백함수
    /// </summary>
    void OpenBox()
    {
        openBtn.image.sprite = btnImages[0];
        closeBtn.image.sprite = btnImages[1];
        nextPage.SetActive(true);
        prevPage.SetActive(true);
        dayText.gameObject.SetActive(true);
        noteController.OpenBox();
    }
    /// <summary>
    /// 상자 닫힘 콜백함수
    /// </summary>
    void CloseBox()
    {
        openBtn.image.sprite = btnImages[1];
        closeBtn.image.sprite = btnImages[0];
    }
    /// <summary>
    /// 제출 버튼 콜백함수
    /// </summary>
    void NewDay()
    {
        blackPanel.gameObject.SetActive(false);
        dayText.text = "Day" + ++dayCount;
    }
}