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
    [SerializeField] Image[] notePanels;
    
    [SerializeField] Image blackPanel;
    [SerializeField] Text dayText;
    [SerializeField] GameObject nextPage;
    [SerializeField] GameObject prevPage;
    [SerializeField] GameObject nextDay;
    [SerializeField] GameObject inventoryUI;
    [SerializeField] GameObject noteBackground_Back;

    Vector2 originalPos;

    bool isOpen = false;
    int dayCount = 1;

    NoteController noteController;


    void Start()
    {
        noteController = GameObject.Find("NoteController").GetComponent<NoteController>();

        originalPos = transform.position;

        for (int i = 0; i < notePanels.Length; i++) 
            notePanels[i].DOFade(0f, 0f);
        blackPanel.DOFade(0f, 0f);
        closeBtn.gameObject.SetActive(false);
        nextPage.SetActive(false);
        prevPage.SetActive(false);
        nextDay.SetActive(false);
        blackPanel.gameObject.SetActive(false);
        dayText.gameObject.SetActive(false);
        noteBackground_Back.SetActive(false);

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
            transform.DOMoveY(originalPos.y + 80f, 0.5f);
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
    public void Open_Anim()
    {
        if (!isOpen)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOMoveY(originalPos.y, 0.5f))
                .Join(boxTop.transform.DOMoveY(960f, 0.5f))
                .Join(boxBottom.transform.DOMoveY(115f, 0.5f))
                .AppendCallback(() =>
                {
                    noteBackground_Back.SetActive(true);
                    for (int i = 0; i < notePanels.Length; i++)
                        notePanels[i].DOFade(1f, 0.5f);
                })
                .OnComplete(() => OpenBox());

            DOTween.Kill(gameObject);
            closeBtn.DOKill();
            openBtn.DOKill();

            isOpen = true;
            openBtn.gameObject.SetActive(false);
            closeBtn.gameObject.SetActive(false);
            sequence.Play();
        }
    }
    /// <summary>
    /// 상자 닫힘 애니메이션
    /// </summary>
    public void Close_Anim()
    {
        if (isOpen)
        {
            noteController.CloseBox();
            nextPage.SetActive(false);
            prevPage.SetActive(false);
            nextDay.SetActive(false);
            dayText.gameObject.SetActive(false);

            Sequence sequence = DOTween.Sequence();
            sequence.AppendCallback(() =>
            {
                for (int i = 0; i < notePanels.Length; i++)
                    notePanels[i].DOFade(0f, 0.5f);
            })
                .AppendInterval(0.5f)
                .Append(boxTop.transform.DOMoveY(10f, 0.5f))
                .Join(boxBottom.transform.DOMoveY(10f, 0.5f))
                .OnComplete(() => CloseBox());

            DOTween.Kill(gameObject);
            closeBtn.DOKill();
            openBtn.DOKill();

            openBtn.gameObject.SetActive(false);
            closeBtn.gameObject.SetActive(false);
            inventoryUI.gameObject.SetActive(false);
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
        sequence.Append(blackPanel.DOFade(1f, 0.5f)).SetEase(Ease.InQuint)
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
        openBtn.gameObject.SetActive(false);
        closeBtn.gameObject.SetActive(true);
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
        isOpen = false;
        openBtn.gameObject.SetActive(true);
        closeBtn.gameObject.SetActive(false);
        noteBackground_Back.SetActive(false);
    }
    /// <summary>
    /// 제출 버튼 콜백함수
    /// </summary>
    void NewDay()
    {
        blackPanel.gameObject.SetActive(false);
        dayText.text = "Day " + ++dayCount;
        MapController.instance.AllowMouseEvent();
    }

    public bool GetIsOpen()
    {
        return isOpen;
    }
}