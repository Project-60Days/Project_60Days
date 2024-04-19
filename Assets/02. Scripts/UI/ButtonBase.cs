using UnityEngine;
using UnityEngine.UI;

public class ButtonBase : MonoBehaviour
{
    [SerializeField] string SFXName_btnClick = "SFX_ButtonClick_01";

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(PlayClickSFX);
    }

    public void PlayClickSFX()
    {
        App.Manager.Sound.PlaySFX(SFXName_btnClick);
    }
}
