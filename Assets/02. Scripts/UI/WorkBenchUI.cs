using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WorkBenchUI : MonoBehaviour
{
    [SerializeField] GameObject inventoryUi;
    [SerializeField] GameObject craftingUi;
    Sequence sequence;
    Vector3 pos1;
    Vector3 pos2;

    private void Start()
    {
        pos1 = inventoryUi.transform.position;
        pos1.x = 2400;

        pos2 = craftingUi.transform.position;
        pos2.x = -500;
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
                craftingUi.SetActive(true);
                inventoryUi.transform.position = pos1;
                craftingUi.transform.position = pos2;
            })
            .AppendInterval(0.5f)
            .Append(inventoryUi.transform.DOMoveX(pos1.x - 1000f, 1f))
            .Join(craftingUi.transform.DOMoveX(pos2.x + 1000f, 1f));
    }
}
