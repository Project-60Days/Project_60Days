using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class HighLight : MonoBehaviour
{
    public string objectID;
    public RectTransform area;
    public GameObject backGround;

    public void Show()
    {
        backGround.SetActive(true);
    }

    public void Hide()
    {
        backGround.SetActive(false);
    }
}
