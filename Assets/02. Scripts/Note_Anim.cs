using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using Yarn.Unity;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Networking.Types;

public class Note_Anim : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button closeBtn;
    public Button openBtn;
    public Button nextPageBtn;
    public Button prevPageBtn;
    public Button nextDayBtn;

    public GameObject boxTop;
    public GameObject boxBottom;
    public GameObject notePanel;
    public GameObject pageContainer;
    public Image blackPanel;
    public Text day;

    public GameObject nextPage;
    public GameObject prevPage;
    public GameObject nextDay;
    public GameObject dialogueBox;

    public Transform[] notePages;
    public Sprite[] btnImages;

    private Vector2 originalPos;
    private Vector2 topOriginalPos;
    private Vector2 bottomOriginalPos;

    private bool isOpen = false;
    private int pageNum = 0;
    private int dayCount = 1;
    int selectedNumber;

    List<int> numbers = new List<int>() { 1, 2, 3, 4, 5 };

    public DialogueRunner dialogueRunner;

    void Start()
    {
        Transform[] allChildren = pageContainer.GetComponentsInChildren<Transform>();
        List<Transform> targets = new List<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.CompareTag("NotePage"))
            {
                targets.Add(child);
            }
        }

        notePages = targets.ToArray();
        for (int i = 0; i < notePages.Length; i++)
        {
            notePages[i].gameObject.SetActive(false);
        }

        notePanel.GetComponent<Image>().DOFade(0f, 0f);
        blackPanel.DOFade(0f, 0f);

        topOriginalPos = boxTop.transform.position;
        bottomOriginalPos = boxBottom.transform.position;
        originalPos = transform.position;

        openBtn.onClick.AddListener(Open_Anim);
        closeBtn.onClick.AddListener(Close_Anim);
        nextPageBtn.onClick.AddListener(NextPageEvent);
        prevPageBtn.onClick.AddListener(PrevPageEvent);
        nextDayBtn.onClick.AddListener(NextDayEvent);

        nextPage.SetActive(false);
        prevPage.SetActive(false);
        nextDay.SetActive(false);
        blackPanel.gameObject.SetActive(false);
        day.gameObject.SetActive(false);
        dialogueBox.SetActive(false);

        int randomIndex = Random.Range(0, numbers.Count);
        selectedNumber = numbers[randomIndex];
        numbers.RemoveAt(randomIndex);
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
            nextPage.SetActive(false);
            prevPage.SetActive(false);
            nextDay.SetActive(false);

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
            day.gameObject.SetActive(false);
            dialogueBox.SetActive(false);
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
        notePages[pageNum].gameObject.SetActive(true);
        day.gameObject.SetActive(true);
        ChangePageButton();

        if (pageNum == 0 || pageNum == 3 || pageNum == 4)
            callYarn(pageNum);
        else
            dialogueBox.SetActive(false);

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
        day.text = "Day" + ++dayCount;
        pageNum = 0;
        int randomIndex = Random.Range(0, numbers.Count);
        selectedNumber = numbers[randomIndex];
        numbers.RemoveAt(randomIndex);
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

        if (pageNum == 0 || pageNum == 3 || pageNum == 4)
            callYarn(pageNum);
        else
            dialogueBox.SetActive(false);
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
            nextDay.SetActive(true);
        }
        else
        {
            nextPageBtn.image.sprite = btnImages[1];
            prevPageBtn.image.sprite = btnImages[1];
            nextDay.SetActive(false);
        }
    }

    /// <summary>
    /// yarn 스크립트 실행 함수
    /// </summary>
    /// <param name="index"></param>
    void callYarn(int index)
    {
        string nodeName;
        dialogueBox.SetActive(true);

        if (index == 0)
        {
            nodeName = "Day" + dayCount;
            dialogueRunner.Stop();
            dialogueRunner.StartDialogue(nodeName);
        }
        else if (index == 3)
        {
            nodeName = "Day" + dayCount + "ChooseEvent";
            dialogueRunner.Stop();
            dialogueRunner.StartDialogue(nodeName);
        }
        else if (index == 4)
        {
            nodeName = "specialEvent" + selectedNumber;
            dialogueRunner.Stop();
            dialogueRunner.StartDialogue(nodeName);
        }
    }
}