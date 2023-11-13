using System.Collections.Generic;
using UnityEngine;

public class SettingController : MonoBehaviour
{
    [SerializeField] GameObject soundDetails;
    [SerializeField] GameObject displayDetails;

    SettingButton[] taps;

    void Awake()
    {
        taps = GetComponentsInChildren<SettingButton>();
    }

    public void OpenSound()
    {
        taps[0].SetButtonClicked();
        taps[1].SetButtonNormal();

        soundDetails.SetActive(true);
        displayDetails.SetActive(false);
    }

    public void OpenDisplay()
    {
        taps[0].SetButtonNormal();
        taps[1].SetButtonClicked();

        soundDetails.SetActive(false);
        displayDetails.SetActive(true);
    }
}
