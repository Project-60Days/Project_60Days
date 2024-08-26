using UnityEngine;

public abstract class SelectBase : MonoBehaviour
{
    [SerializeField] Sprite imageA;
    [SerializeField] Sprite imageB;

    public abstract string Key();

    protected abstract string GetTextA();
    protected abstract string GetTextB();

    public virtual void SetOptionA(SelectButton _select)
    {
        _select.btn.onClick.RemoveAllListeners();
        _select.btn.onClick.AddListener(SelectA);

        _select.img.sprite = imageA;
        _select.text.text = GetTextA();
    }

    public virtual void SetOptionB(SelectButton _select)
    {
        _select.btn.onClick.RemoveAllListeners();
        _select.btn.onClick.AddListener(SelectB);

        _select.img.sprite = imageB;
        _select.text.text = GetTextB();
    }

    protected virtual void SelectA()
    {
        App.Manager.UI.GetPanel<SelectPanel>().ClosePanel();
    }

    protected virtual void SelectB()
    {
        App.Manager.UI.GetPanel<SelectPanel>().ClosePanel();
    }
}
