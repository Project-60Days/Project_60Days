using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class WorkBenchUiOpen : MonoBehaviour
{
    [SerializeField] GameObject inventoryUi;
    [SerializeField] GameObject craftingUi;

    

    void Start()
    {
        inventoryUi.GetComponent<CanvasGroup>().alpha = 0.0f;
        craftingUi.GetComponent<CanvasGroup>().alpha = 0.0f;

        ActivateUiObjects(false);
    }

    void ActivateUiObjects(bool _isActive)
    {
        inventoryUi.SetActive(_isActive);
        craftingUi.SetActive(_isActive);
    }

    void OnEnable()
    {
        SetOnClickEvent(true);
    }

    void OnDisable()
    {
        SetOnClickEvent(false);
    }

    void SetOnClickEvent(bool _enable)
    {
        WorkBenchInteraction onClickScript = FindObjectOfType<WorkBenchInteraction>();
        if (onClickScript != null)
        {
            if (_enable == true)
            {
                onClickScript.onClickEvent.AddListener(OpenUi);
            }
            else
            {
                onClickScript.onClickEvent.RemoveListener(OpenUi);
            }
        }
    }

    void OpenUi()
    {
        UIManager.instance.AddCurrUIName(StringUtility.UI_CRAFTING);
        ActivateUiObjects(true);
        FadeInUiObjects();
    }

    void FadeInUiObjects()
    {
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(craftingUi.GetComponent<CanvasGroup>().DOFade(1f, 0.5f))
            .Append(inventoryUi.GetComponent<CanvasGroup>().DOFade(1f, 0.5f));
        sequence.Play();
    }

    public void CloseUi()
    {
        Sequence sequence = DOTween.Sequence();
        UIManager.instance.PopCurrUI();
        sequence
            .Append(inventoryUi.GetComponent<CanvasGroup>().DOFade(0f, 0.5f))
            .Append(craftingUi.GetComponent<CanvasGroup>().DOFade(0f, 0.5f))
            .OnComplete(() => ActivateUiObjects(false));
        sequence.Play();
    }
}
