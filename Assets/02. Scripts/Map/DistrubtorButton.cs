using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DistrubtorButton : MonoBehaviour
{
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(Distrubtor);
    }

    public void Distrubtor()
    {
        if (UIManager.instance.GetInventoryController().CheckDistrubtorUsage())
        {
            Debug.Log("교란기 있음");
            if (App.instance.GetMapManager().CheckCanInstallDrone())
            {
                Debug.Log("교란기 설치 가능");
                App.instance.GetMapManager().mapController.PreparingDistrubtor(true);
            }
        }
        else
        {
            return;
        }
    }
}
