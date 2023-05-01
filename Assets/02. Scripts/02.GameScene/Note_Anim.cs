using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class Note_Anim : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button closeBtn;
    public Button openBtn;
    public Button nextPageBtn;
    public Button prevPageBtn;

    public GameObject boxTop;
    public GameObject boxBottom;
    public GameObject notePanel;
    public GameObject pageContainer;

    public Text[] notePages;
    public Sprite[] btnImages;

    private Vector2 originalPos;
    private Vector2 topOriginalPos;
    private Vector2 bottomOriginalPos;

    private bool isOpen = false;
    private int pageNum = 0;

    void Start()
    {
        notePages = pageContainer.GetComponentsInChildren<Text>(true);
        for (int i = 0; i < notePages.Length; i++)
        {
            notePages[i].gameObject.SetActive(false);
        }

        notePanel.GetComponent<Image>().DOFade(0f, 0f);

        topOriginalPos = boxTop.transform.position;
        bottomOriginalPos = boxBottom.transform.position;
        originalPos = transform.position;

        openBtn.onClick.AddListener(Open_Anim);
        closeBtn.onClick.AddListener(Close_Anim);
        nextPageBtn.onClick.AddListener(NextPageEvent);
        prevPageBtn.onClick.AddListener(PrevPageEvent);
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
    public void Open_Anim()
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
            nextPageBtn.image.sprite = btnImages[0];
            prevPageBtn.image.sprite = btnImages[0];
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
            nextPageBtn.image.sprite = btnImages[0];
            prevPageBtn.image.sprite = btnImages[0];
            notePages[pageNum].gameObject.SetActive(false);
            sequence.Play();
        }
    }

    /// <summary>
    /// 상자 열림 콜백함수
    /// </summary>
    void OpenBox()
    {
        openBtn.image.sprite = btnImages[0];
        closeBtn.image.sprite = btnImages[1];
        notePages[pageNum].gameObject.SetActive(true);
        ChangePageButton();
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
    /// 다음 페이지 버튼
    /// </summary>
    private void NextPageEvent()
    {
        if (pageNum + 1 > notePages.Length - 1)
            return;

        PageOn(pageNum + 1);
    }
    /// <summary>
    /// 이전 페이지 버튼
    /// </summary>
    private void PrevPageEvent()
    {
        if (pageNum - 1 < 0)
            return;

        PageOn(pageNum - 1);
    }

    private void PageOn(int index)
    {
        notePages[pageNum].gameObject.SetActive(false);
        notePages[index].gameObject.SetActive(true);

        pageNum = index;

        ChangePageButton();
    }

    /// <summary>
    /// 페이지 버튼 이미지 변경
    /// </summary>
    void ChangePageButton()
    {
        if (pageNum == 0)
        {
            nextPageBtn.image.sprite = btnImages[1];
            prevPageBtn.image.sprite = btnImages[0];
        }
        else if (pageNum == notePages.Length - 1)
        {
            nextPageBtn.image.sprite = btnImages[0];
            prevPageBtn.image.sprite = btnImages[1];
        }
        else
        {
            nextPageBtn.image.sprite = btnImages[1];
            prevPageBtn.image.sprite = btnImages[1];
        }
    }
}
