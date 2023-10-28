using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MenuWithTap : MenuButtonBase
{
    GameObject backBtn;
    GameObject detailsBack;

    float initialY;
    int hierarchyIndex;

    public override void Set()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        buttonImage = GetComponent<Image>();

        backBtn = transform.Find("Back_Btn").gameObject;
        detailsBack = transform.GetChild(2).gameObject;

        hierarchyIndex = transform.GetSiblingIndex();
        initialY = transform.position.y;
    }
    void SetChildrenStatus(bool _isOpen)
    {
        isClicked = _isOpen;
        backBtn.SetActive(_isOpen);
        detailsBack.SetActive(_isOpen);
    }

    public override void Init()
    {
        SetButtonNormal();
        SetChildrenStatus(false);
    }

    public override void ClickEvent()
    {
        if (isClicked == false)
        {
            SetChildrenStatus(true);
            transform.SetAsLastSibling();
            gameObject.GetComponent<Transform>().DOLocalMoveY(231f, 0.3f);
        }
    }
    
    

    public void CloseEvent()
    {
        SetChildrenStatus(false);
        gameObject.GetComponent<Transform>().DOMoveY(initialY, 0.3f).OnComplete(() =>
        {
            transform.SetSiblingIndex(hierarchyIndex);
            SetButtonNormal();
        });
    }
}
