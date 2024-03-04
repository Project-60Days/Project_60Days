using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExplorerButton : MonoBehaviour
{
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(Explorer);
    }

    public void Explorer()
    {
        if (UIManager.instance.GetInventoryController().CheckFindorUsage())
        {
            Debug.Log("탐색기 있음");
            if (App.instance.GetMapManager().CheckCanInstallDrone())
            {
                Debug.Log("탐색기 설치 가능");

                App.instance.GetMapManager().mapController.PreparingExplorer(true);
            }
        }
        else
        {
            return;
        }
    }
}
