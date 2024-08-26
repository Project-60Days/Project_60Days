using UnityEngine;
using DG.Tweening;

public class MenuDetails : MenuBase
{
    [SerializeField] GameObject backBtn;
    [SerializeField] GameObject detailsBack;

    private int hierarchyIndex;

    protected override void Set()
    {
        base.Set();

        hierarchyIndex = transform.GetSiblingIndex();
    }

    public override void Init()
    {
        base.Init();

        SetChildrenState(false);
    }

    private void SetChildrenState(bool _isOpen)
    {
        IsClicked = _isOpen;
        backBtn.SetActive(_isOpen);
        detailsBack.SetActive(_isOpen);
    }

    protected override void ClickEvent()
    {
        transform.SetAsLastSibling();
        transform.DOLocalMoveY(209f, 0.3f)
            .OnComplete(() =>
            {
                SetChildrenState(true);
            });
    }

    public override void CloseEvent()
    {
        SetChildrenState(false);
        transform.DOLocalMoveY(startPositionY, 0.3f)
            .OnComplete(() =>
            {
                transform.SetSiblingIndex(hierarchyIndex);
                SetButtonState(false);
            });
    }
}
