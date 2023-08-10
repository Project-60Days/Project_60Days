using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisturbanceButton : MonoBehaviour
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
        gameObject.GetComponent<Button>().onClick.AddListener(Disturbance);
    }

    public void Disturbance()
    {
        if(controller.CheckSelected())
            controller.DistrubtorBorderActiveSet(true);
    }
}
