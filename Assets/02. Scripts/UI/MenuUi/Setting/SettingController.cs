using System.Collections.Generic;
using UnityEngine;

public class SettingController : MonoBehaviour
{
    [SerializeField] SettingButton soundTap;
    [SerializeField] SettingButton displayTap;
    [SerializeField] GameObject soundDetails;
    [SerializeField] GameObject displayDetails;

    public void OpenSound()
    {
        soundTap.SetButtonClicked();
        displayTap.SetButtonNormal();

        soundDetails.SetActive(true);
        displayDetails.SetActive(false);
    }

    public void OpenDisplay()
    {
        soundTap.SetButtonNormal();
        displayTap.SetButtonClicked();

        soundDetails.SetActive(false);
        displayDetails.SetActive(true);
    }
}
