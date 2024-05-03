using UnityEngine;
using UnityEngine.UI;

public class ButtonBase : MonoBehaviour
{
    [SerializeField] string SFXName_btnClick = "SFX_Button_1";

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(PlayClickSFX);
    }

    public void PlayClickSFX()
    {
        App.Manager.Sound.PlaySFX(SFXName_btnClick);
    }
}
