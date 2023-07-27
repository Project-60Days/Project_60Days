using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndUIController : MonoBehaviour
{
    [SerializeField] GameObject imageBack;

    public void Show()
    {
        imageBack.SetActive(true);
    }

    public void Hide()
    {
        imageBack.SetActive(false);
    }

}
