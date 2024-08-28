using UnityEngine;
using DG.Tweening;
using TMPro;
using DG.Tweening;

public class InfoPanel : UIBase
{
    [SerializeField] RectTransform rect;
    [SerializeField] TextMeshProUGUI text;

    private CanvasGroup canvasGroup;

    private bool isDescriptionOn;

    #region Override
    public override void Init()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();

        ClosePanel();
    }

    public override void ClosePanel()
    {
        canvasGroup.DOKill();

        isDescriptionOn = false;

        ResetUI();
    }
    #endregion

    private void Update()
    {
        if (isDescriptionOn)
        {
            UpdateDescriptionTransform();
        }
    }

    private void UpdateDescriptionTransform()
    {
        Vector2 position = Input.mousePosition;

        position.x = Mathf.Min(position.x, Screen.width - rect.rect.width);
        position.y = Mathf.Min(position.y, Screen.height - rect.rect.height);

        rect.position = position;
    }

    public void SetInfo(string _text)
    {
        canvasGroup.DOKill();

        canvasGroup.DOFade(0f, 0.1f).OnComplete(() =>
        {
            isDescriptionOn = true;

            ResetUI();

            text.DOText(_text, 0.5f, true, ScrambleMode.Uppercase);

            canvasGroup.DOFade(1f, 0.1f);
        });
    }

    private void ResetUI()
    {
        canvasGroup.alpha = 0f;

        text.text = "";
    }
}
