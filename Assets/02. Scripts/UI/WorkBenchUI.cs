using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WorkBenchUI : MonoBehaviour
{
    [SerializeField] GameObject inventoryUi;
    [SerializeField] GameObject craftingUi;

    [SerializeField] Transform craftingStartPos;
    [SerializeField] Transform craftingPointPos;

    [SerializeField] Transform inventoryStartPos;
    [SerializeField] Transform inventoryPointPos;

    Sequence sequence;

    private void Start()
    {
        inventoryUi.transform.position = craftingStartPos.position;
        craftingUi.transform.position = inventoryStartPos.position;

    }

    private void OnEnable()
    {
        WorkBench onClickScript = FindObjectOfType<WorkBench>();
        if (onClickScript != null)
        {
            onClickScript.onClickEvent.AddListener(ActivateObject);
        }
    }

    private void OnDisable()
    {
        WorkBench onClickScript = FindObjectOfType<WorkBench>();
        if (onClickScript != null)
        {
            onClickScript.onClickEvent.RemoveListener(ActivateObject);
        }
    }

    private void ActivateObject()
    {
        sequence = DOTween.Sequence();

        sequence
            .OnStart(() =>
            {
                inventoryUi.SetActive(true);
                craftingUi.SetActive(true);
                inventoryUi.transform.position = inventoryStartPos.position;
                craftingUi.transform.position = craftingStartPos.position;
            })
            .Append(inventoryUi.transform.DOMoveX(inventoryPointPos.position.x, 1f))
            .Join(craftingUi.transform.DOMoveX(craftingPointPos.position.x, 1f));
    }
}
