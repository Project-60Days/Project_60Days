using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisturbanceButton : MonoBehaviour
{
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(Disturbance);
    }

    public void Disturbance()
    {
        if (UIManager.instance.GetInventoryController().CheckDisturbeUsage())
        {
            Debug.Log("교란기 있음");
            if (App.instance.GetMapManager().CheckCanInstallDrone())
            {
                Debug.Log("교란기 설치 가능");
                App.instance.GetMapManager().mapController.PreparingDisturbtor(true);
            }
        }
        else
        {
            return;
        }
    }
}
