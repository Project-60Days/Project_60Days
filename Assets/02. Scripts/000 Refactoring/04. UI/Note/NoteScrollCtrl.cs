using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NoteScrollCtrl : MonoBehaviour
{
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] RectTransform content;

    private RectTransform rect;
    private float startPositionY;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        startPositionY = rect.anchoredPosition.y;

        scrollRect.onValueChanged.AddListener(OnScrollValueChanged);

        StartAnim();
    }

    private void StartAnim()
    {
        rect.DOAnchorPosY(startPositionY + 10f, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }

    private void StopAnim()
    {
        transform.DOKill();

        transform.DOMoveY(startPositionY, 0f)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
    }

    public void ResetScrollActive()
    {
        bool canScroll = content.rect.height > scrollRect.viewport.rect.height;
        gameObject.SetActive(canScroll);
    }

    private void OnScrollValueChanged(Vector2 scrollPosition)
    {
        if (scrollRect.verticalNormalizedPosition <= 0.1f)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
