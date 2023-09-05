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

    void OnEnable()
    {
        SetOnClickEvent(true);
    }

    void OnDisable()
    {
        SetOnClickEvent(false);
    }

    void SetOnClickEvent(bool enable)
    {
        WorkBenchInteraction onClickScript = FindObjectOfType<WorkBenchInteraction>();
        if (onClickScript != null)
        {
            if (enable)
            {
                onClickScript.onClickEvent.AddListener(UiOpenEvent);
            }
            else
            {
                onClickScript.onClickEvent.RemoveListener(UiOpenEvent);
            }
        }
    }

    void UiOpenEvent()
    {
        UIManager.instance.AddCurrUIName(StringUtility.UI_CRAFTING);
        ActivateUiObjects(true);
        FadeInUiObjects();
    }

    public void UiCloseEvent()
    {
        Sequence sequence = DOTween.Sequence();
        UIManager.instance.PopCurrUI();
        sequence
            .Append(inventoryUi.GetComponent<CanvasGroup>().DOFade(0f, 0.5f))
            .Append(craftingUi.GetComponent<CanvasGroup>().DOFade(0f, 0.5f))
            .OnComplete(() => ActivateUiObjects(false));
        sequence.Play();
    }

    void ActivateUiObjects(bool isActive)
    {
        Debug.Log("d");
        inventoryUi.SetActive(isActive);
        craftingUi.SetActive(isActive);
    }

    void FadeInUiObjects()
    {
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(craftingUi.GetComponent<CanvasGroup>().DOFade(1f, 0.5f))
            .Append(inventoryUi.GetComponent<CanvasGroup>().DOFade(1f, 0.5f));
        sequence.Play();
    }

    public void Temp() //테스트용 버튼에 쓰일 임시 함수
    {
        ActivateUiObjects(true);
        FadeInUiObjects();
    }
}
