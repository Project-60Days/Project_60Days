using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InfoController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

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
        text.gameObject.SetActive(false);
        
        gameObject.SetActive(false);
    }

    public void ShowAlertInfo(string _text, Vector3 _mouseCoordinate)
    {
        if (isNew == true)
        {
            HideInfo();
            SetObjects(_text);
            isNew = false;
        }

        SetTransform(_mouseCoordinate);

        gameObject.SetActive(true);
    }

    public void ShowMapInfo(ETileType _type, Vector3 _mouseCoordinate)
    {
        if (isNew == true)
        {
            HideInfo();
            SetObjects(_type);
            isNew = false;
        }

        SetTransform(_mouseCoordinate);

        gameObject.SetActive(true);
    }

    void SetObjects(string _text)
    {
        if (_text != null)
        {
            text.gameObject.SetActive(true);
            text.text = _text;
        }
    }

    void SetObjects(ETileType _type)
    {
        string type = _type.ToString().ToUpper();
        string code = "STR_TILE_" + type + "_DESCRIPTION";
  
        text.gameObject.SetActive(true);
        text.text = App.instance.GetDataManager().GetString(code);
    }

    void SetTransform(Vector3 _mouseCoordinate)
    {
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
    }
}
