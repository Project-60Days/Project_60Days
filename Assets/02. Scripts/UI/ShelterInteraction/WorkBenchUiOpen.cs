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
        if (App.Manager.UI.isUIStatus(UIState.Normal) == false) return;

        //ActivateUiObjects(true);
        App.Manager.UI.GetPanel<CraftPanel>().OpenPanel();
        craftEffectAnim.Init();

        FadeInUiObjects();
    }

    void FadeInUiObjects()
    {
        App.Manager.Sound.PlaySFX("SFX_SceneChange_BaseToCrafting");
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(craftingUi.GetComponent<CanvasGroup>().DOFade(1f, 0.5f))
            .Append(inventoryUi.GetComponent<CanvasGroup>().DOFade(1f, 0.5f))
            .Join(productionUi.GetComponent<CanvasGroup>().DOFade(1f, 0.5f))
            .Join(decorationUi.GetComponent<CanvasGroup>().DOFade(1f, 0.5f))
            .OnComplete(() => App.Manager.UI.GetItemInfoController().isOpen = true);
    }

    public void CloseUi()
    {
        craftEffectAnim.isActive = false;
        App.Manager.UI.GetItemInfoController().isOpen = false;
        //App.Manager.UI.GetPanel<CraftPanel>().ClosePanel();

        FadeOutUiObjects();
    }

    void FadeOutUiObjects()
    {
        App.Manager.Sound.PlaySFX("SFX_SceneChange_CraftingToBase");
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(inventoryUi.GetComponent<CanvasGroup>().DOFade(0f, 0.5f))
            .Join(productionUi.GetComponent<CanvasGroup>().DOFade(0f, 0.5f))
            .Join(decorationUi.GetComponent<CanvasGroup>().DOFade(0f, 0.5f))
            .Append(craftingUi.GetComponent<CanvasGroup>().DOFade(0f, 0.5f))
            .OnComplete(() => ActivateUiObjects(false));
    }
}
