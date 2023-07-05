using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExplorerButton : MonoBehaviour
{
    MapController controller;
    void Start()
    {
        StartCoroutine(GetMapController());
    }

    IEnumerator GetMapController()
    {
        yield return new WaitForEndOfFrame();
        controller = GameObject.FindGameObjectWithTag("MapController").GetComponent<MapController>();
        gameObject.GetComponent<Button>().onClick.AddListener(Explorer);
    }

    public void Explorer()
    {
        Debug.Log("탐색기 버튼 눌림!");
        controller.ExplorerBorderActiveOn();
    }
}
