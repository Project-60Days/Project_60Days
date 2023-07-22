using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkBenchUI : MonoBehaviour
{
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
        Debug.Log("гоюл");
    }
}
