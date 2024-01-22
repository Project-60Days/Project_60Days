using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AlertInfoController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI alertText;

    public bool isNew = true;

    RectTransform infoTransform;

    void Awake()
    {
        infoTransform = gameObject.GetComponent<RectTransform>();

        HideInfo();
    }

    public void HideInfo()
    {
        InitObjects();
    }

    void InitObjects()
    {
        alertText.gameObject.SetActive(false);
        
        gameObject.SetActive(false);
    }

    public void ShowInfo(string _text, Vector3 _mouseCoordinate)
    {
        if (isNew == true)
        {
            HideInfo();
            SetObejcts(_text);
            isNew = false;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(infoTransform);

        float width = infoTransform.rect.width;
        float height = infoTransform.rect.height;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float newX = _mouseCoordinate.x;
        float newY = _mouseCoordinate.y;

        if (newX + width > screenWidth * 0.95)
            newX -= width * (screenWidth / 1920);
        if (newY - height < screenHeight * 0.1)
            newY += height * (screenHeight / 1080);

        infoTransform.position = new Vector3(newX, newY, infoTransform.position.z);

        gameObject.SetActive(true);
    }

    void SetObejcts(string _text)
    {
        if (_text != null)
        {
            alertText.gameObject.SetActive(true);
            alertText.text = _text;
        }
    }
}
