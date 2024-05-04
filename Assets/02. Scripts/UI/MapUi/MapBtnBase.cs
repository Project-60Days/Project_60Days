using UnityEngine.UI;

public abstract class MapBtnBase : IconBase
{
    protected override void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(OnClickEvent);
    }

    protected abstract void OnClickEvent();
}
