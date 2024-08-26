using UnityEngine.UI;

public abstract class InstallBase : IconBase
{
    protected override void Start()
    {
        base.Start();

        gameObject.GetComponent<Button>().onClick.AddListener(OnClickEvent);
    }

    protected abstract void OnClickEvent();
}
