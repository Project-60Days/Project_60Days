using UnityEngine;
using DG.Tweening;
using TMPro;

public class InfoPanel : UIBase
{
    [SerializeField] RectTransform rect;
    [SerializeField] TextMeshProUGUI text;

    private bool isDescriptionOn = false;

    #region Override
    public override void Init()
    {
        gameObject.SetActive(false);
    }

    public override void ReInit() { }

    public override void ClosePanel()
    {
        base.ClosePanel();

        isDescriptionOn = false;
        text.text = "";
    }
    #endregion

    public void SetInfo(string _text)
    {
        base.OpenPanel();

        isDescriptionOn = true;
        text.DOText(_text, 0.5f, true, ScrambleMode.Uppercase);
    }

    void Update()
    {
        if (isDescriptionOn)
        {
            UpdateDescriptionTransform();
        }
    }

    private void UpdateDescriptionTransform()
    {
        Vector2 position = Input.mousePosition;

        if (position.x + rect.rect.width > Screen.width)
        {
            position.x = Screen.width - rect.rect.width;
        }

        if (position.y + rect.rect.height > Screen.height)
        {
            position.y = Screen.height - rect.rect.height;
        }

        rect.position = position;
    }
}
