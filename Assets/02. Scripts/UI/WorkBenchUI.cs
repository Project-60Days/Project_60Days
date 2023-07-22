using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WorkBenchUI : MonoBehaviour
{
    [SerializeField] GameObject inventoryUi;
    Sequence sequence;
    Vector3 pos;

    private void Start()
    {
        pos = inventoryUi.transform.position;
        pos.x = 2400;
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
            .OnStart(() => {
                inventoryUi.SetActive(true);
                inventoryUi.transform.position = pos;
            })
            .AppendInterval(0.5f)
            .Append(inventoryUi.transform.DOMoveX(pos.x - 1000f, 1f));
    }
}
