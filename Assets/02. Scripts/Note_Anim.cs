using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using DG.Tweening.Core;
//using System.Linq;

public class Note_Anim : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button closeBtn;
    public Button openBtn;
    public Button nextPageBtn;
    public Button prevPageBtn;

    public GameObject boxTop;
    public GameObject boxBottom;
    public GameObject notePanel;

    private bool isOpen = false;

    public Sprite[] btnImages;

    private Vector2 originalPos;
    private Vector2 topOriginalPos;
    private Vector2 bottomOriginalPos;

    void Start()
    {
        notePanel.GetComponent<Image>().DOFade(0f, 0f);

        topOriginalPos = boxTop.transform.position;
        bottomOriginalPos = boxBottom.transform.position;
        originalPos = transform.position;

        openBtn.onClick.AddListener(Open_Anim);
        closeBtn.onClick.AddListener(Close_Anim);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!isOpen)
            transform.DOMoveY(originalPos.y + 50f, 1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!isOpen)
            transform.DOMoveY(originalPos.y, 1f);
    }
    
    public void Open_Anim()
    {
        isOpen = true;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(boxTop.transform.DOMoveY(topOriginalPos.y + 900f, 1f))
            .Join(boxBottom.transform.DOMoveY(bottomOriginalPos.y + 150f, 1f))
            .Join(transform.DOMoveY(originalPos.y, 1f))
            .Append(notePanel.GetComponent<Image>().DOFade(1f, 0.5f));

        DOTween.Kill(gameObject);
        closeBtn.DOKill();
        openBtn.DOKill();

        sequence.Play();
        openBtn.image.sprite = btnImages[0];
        closeBtn.image.sprite = btnImages[1];
    }

    public void Close_Anim()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(notePanel.GetComponent<Image>().DOFade(0f, 0.5f))
            .Append(boxTop.transform.DOMoveY(topOriginalPos.y, 1f))
            .Join(boxBottom.transform.DOMoveY(bottomOriginalPos.y, 1f));

        DOTween.Kill(gameObject);
        closeBtn.DOKill();
        openBtn.DOKill();

        sequence.Play();
        isOpen = false;
        openBtn.image.sprite = btnImages[1];
        closeBtn.image.sprite = btnImages[0];
    }
}
