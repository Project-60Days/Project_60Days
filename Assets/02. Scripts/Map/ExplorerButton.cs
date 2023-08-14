using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExplorerButton : MonoBehaviour
{
    MapManager controller;
    void Start()
    {
        StartCoroutine(GetMapController());
    }

    IEnumerator GetMapController()
    {
        yield return new WaitForEndOfFrame();
        controller = GameObject.FindGameObjectWithTag("MapController").GetComponent<MapManager>();
        gameObject.GetComponent<Button>().onClick.AddListener(Explorer);
    }

    public void Explorer()
    {
        if(controller.CheckSelected())
            controller.ExplorerMachineSettting(true);
    }
}
