using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MenuWithTap : MenuButtonBase
{
    [SerializeField] GameObject backBtn;
    [SerializeField] GameObject detailsBack;

    int hierarchyIndex;

    public override void Set()
    {
        base.Set();

        hierarchyIndex = transform.GetSiblingIndex();
    }

    void SetChildrenStatus(bool _isOpen)
    {
        isClicked = _isOpen;
        backBtn.SetActive(_isOpen);
        detailsBack.SetActive(_isOpen);
    }

    public override void Init()
    {
        base.Init();

        SetChildrenStatus(false);
    }

    public override void ClickEvent()
    {
        transform.SetAsLastSibling();
        transform.DOLocalMoveY(209f, 0.3f).OnComplete(() =>
        {
            SetChildrenStatus(true);
        });
    }

    public override void CloseEvent()
    {
        SetChildrenStatus(false);
        transform.DOLocalMoveY(startPositionY, 0.3f).OnComplete(() =>
        {
            transform.SetSiblingIndex(hierarchyIndex);
            SetButtonState(false);
        });
    }
}
