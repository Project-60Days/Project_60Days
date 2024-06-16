using UnityEngine;
using UnityEngine.EventSystems;

public abstract class IconBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    protected string text;

    private bool isMouseEnter = false;
    private InfoPanel Info;

    protected virtual void Start()
    {
        Info = App.Manager.UI.GetPanel<InfoPanel>();
        text = GetString();
    }

    protected abstract string GetString();

    private void DescriptionOn()
    {
        if (isMouseEnter) return;

        Info.SetInfo(text);
        isMouseEnter = true;
    }

    private void DescriptionOff()
    {
        if (!isMouseEnter) return;

        Info.ClosePanel();
        isMouseEnter = false;
    }

    #region Set Event
    public void OnPointerEnter(PointerEventData eventData)
    {
        DescriptionOn();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DescriptionOff();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        DescriptionOff();
    }
    #endregion
}