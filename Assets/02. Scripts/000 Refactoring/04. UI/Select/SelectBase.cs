using UnityEngine;

public abstract class SelectBase : MonoBehaviour
{
    public string Key { get; protected set; }

    [SerializeField] Sprite imageA;
    [SerializeField] Sprite imageB;

    [SerializeField] string textA;
    [SerializeField] string textB;

    public virtual void SetOptionA(SelectButton _select)
    {
        _select.btn.onClick.RemoveAllListeners();
        _select.btn.onClick.AddListener(SelectA);

        _select.img.sprite = imageA;
        _select.text.text = textA;
    }

    public virtual void SetOptionB(SelectButton _select)
    {
        _select.btn.onClick.RemoveAllListeners();
        _select.btn.onClick.AddListener(SelectB);

        _select.img.sprite = imageB;
        _select.text.text = textB;
    }

    public virtual void SelectA()
    {
        App.Manager.UI.GetPanel<SelectPanel>().ClosePanel();
    }

    public virtual void SelectB()
    {
        App.Manager.UI.GetPanel<SelectPanel>().ClosePanel();
    }
}
