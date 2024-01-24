using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class WorkBenchUiOpen : MonoBehaviour
{
    [SerializeField] GameObject inventoryUi;
    [SerializeField] GameObject craftingUi;
    [SerializeField] GameObject productionUi;
    [SerializeField] GameObject decorationUi;
    [SerializeField] Button closeBtn;
    CraftEffectAnim craftEffectAnim;
    

    void Start()
    {
        inventoryUi.GetComponent<CanvasGroup>().alpha = 0.0f;
        craftingUi.GetComponent<CanvasGroup>().alpha = 0.0f;
        productionUi.GetComponent<CanvasGroup>().alpha = 0.0f;
        decorationUi.GetComponent<CanvasGroup>().alpha = 0.0f;

        craftEffectAnim = craftingUi.GetComponentInChildren<CraftEffectAnim>();

        ActivateUiObjects(false);
    }

    void ActivateUiObjects(bool _isActive)
    {
        inventoryUi.SetActive(_isActive);
        craftingUi.SetActive(_isActive);
        productionUi.SetActive(_isActive);
        decorationUi.SetActive(_isActive);
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
        if (UIManager.instance.isUIStatus("UI_NORMAL") == false) return;

        UIManager.instance.AddCurrUIName(StringUtility.UI_CRAFTING);

        ActivateUiObjects(true);
        UIManager.instance.GetCraftingUiController().EnterUi();
        craftEffectAnim.Init();

        FadeInUiObjects();

        closeBtn.enabled = true;
    }

    void FadeInUiObjects()
    {
        App.instance.GetSoundManager().PlaySFX("SFX_SceneChange_BaseToCrafting");
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(craftingUi.GetComponent<CanvasGroup>().DOFade(1f, 0.5f))
            .Append(inventoryUi.GetComponent<CanvasGroup>().DOFade(1f, 0.5f))
            .Join(productionUi.GetComponent<CanvasGroup>().DOFade(1f, 0.5f))
            .Join(decorationUi.GetComponent<CanvasGroup>().DOFade(1f, 0.5f))
            .OnComplete(() => UIManager.instance.GetItemInfoController().isOpen = true);
    }

    public void CloseUi()
    {
        closeBtn.enabled = false;

        UIManager.instance.PopCurrUI();

        craftEffectAnim.isActive = false;
        UIManager.instance.GetItemInfoController().isOpen = false;
        UIManager.instance.GetCraftingUiController().ExitUi();

        FadeOutUiObjects();
    }

    void FadeOutUiObjects()
    {
        App.instance.GetSoundManager().PlaySFX("SFX_SceneChange_CraftingToBase");
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(inventoryUi.GetComponent<CanvasGroup>().DOFade(0f, 0.5f))
            .Join(productionUi.GetComponent<CanvasGroup>().DOFade(0f, 0.5f))
            .Join(decorationUi.GetComponent<CanvasGroup>().DOFade(0f, 0.5f))
            .Append(craftingUi.GetComponent<CanvasGroup>().DOFade(0f, 0.5f))
            .OnComplete(() => ActivateUiObjects(false));
    }
}
