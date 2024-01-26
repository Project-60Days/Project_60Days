using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class InfoController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    

    public bool isNew = true;

    RectTransform infoTransform;
    CanvasGroup canvasGroup;

    void Awake()
    {
        infoTransform = gameObject.GetComponent<RectTransform>();
        canvasGroup = gameObject.GetComponent<CanvasGroup>();

        HideInfo();
    }

    public void HideInfo()
    {
        text.gameObject.SetActive(false);
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    public void ShowAlertInfo(string _code, Vector3 _mouseCoordinate)
    {
        if (isNew == true)
        {
            HideInfo();
            SetObjects(_code);
            isNew = false;
        }

        SetTransform(_mouseCoordinate);

        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;
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
        canvasGroup.alpha = 1f;
    }

    void SetObjects(string _code)
    {
        string textString = App.instance.GetDataManager().GetString(_code);
        text.gameObject.SetActive(true);
        text.DOText(textString, 0.5f, true, ScrambleMode.Uppercase);
    }

    void SetObjects(ETileType _type)
    {
        string type = _type.ToString().ToUpper();
        string code = "STR_TILE_" + type + "_DESCRIPTION";
        string textString = App.instance.GetDataManager().GetString(code);

        text.gameObject.SetActive(true);
        text.DOText(textString, 0.5f, true, ScrambleMode.Numerals);
    }

    void SetTransform(Vector3 _mouseCoordinate)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(infoTransform);

        float newX = _mouseCoordinate.x;
        float newY = _mouseCoordinate.y;

        infoTransform.position = new Vector3(newX, newY, infoTransform.position.z);
    }
}
