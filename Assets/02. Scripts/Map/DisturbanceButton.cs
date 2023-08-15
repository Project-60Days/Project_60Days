using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisturbanceButton : MonoBehaviour
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
        gameObject.GetComponent<Button>().onClick.AddListener(Disturbance);
    }

    public void Disturbance()
    {
        if(controller.CheckSelected())
            controller.PreparingDisturbtor(true);
    }
}
