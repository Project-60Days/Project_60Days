using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class WorkBenchUiOpen : MonoBehaviour
{
    [SerializeField] GameObject inventoryUi;
    [SerializeField] GameObject craftingUi;
    [SerializeField] GameObject blackPanel;

    Sequence sequence;

    private void Start()
    {
        inventoryUi.GetComponent<CanvasGroup>().alpha = 0.0f;
        craftingUi.GetComponent<CanvasGroup>().alpha = 0.0f;
        craftingUi.SetActive(false);
        inventoryUi.SetActive(false);
        blackPanel.SetActive(false);
    }

    private void OnEnable()
    {
        SetOnClickEvent(true);
    }

    private void OnDisable()
    {
        SetOnClickEvent(false);
    }

    private void SetOnClickEvent(bool enable)
    {
        WorkBenchInteraction onClickScript = FindObjectOfType<WorkBenchInteraction>();
        if (onClickScript != null)
        {
            if (enable)
            {
                onClickScript.onClickEvent.AddListener(ActivateUIObjects);
            }
            else
            {
                onClickScript.onClickEvent.RemoveListener(ActivateUIObjects);
            }
        }
    }

    private void ActivateUIObjects()
    {
        sequence?.Kill();
        craftingUi.SetActive(true);
        inventoryUi.SetActive(true);
        blackPanel.SetActive(true);
        sequence = DOTween.Sequence();

        sequence
            .Append(craftingUi.GetComponent<CanvasGroup>().DOFade(1f, 0.5f))
            .Append(inventoryUi.GetComponent<CanvasGroup>().DOFade(1f, 0.5f));

        sequence.Play();
    }
}
