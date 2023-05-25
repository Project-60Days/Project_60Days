using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MapCamera : MonoBehaviour
{
    GameObject player;
    CinemachineVirtualCamera camera;

    private void Start()
    {
        StartCoroutine(GetCamera());
        camera = gameObject.GetComponent<CinemachineVirtualCamera>();
    }

    public IEnumerator GetCamera()
    {
        yield return new WaitForEndOfFrame();
        player = GameObject.FindGameObjectWithTag("Player");
        camera.Follow = player.transform;
    }

    public void SetPrioryty(bool isOn)
    {
        if (isOn)
        {
            camera.Priority = 11;
            Hexamap.DemoController.instance.BaseOnOff(false);
        }
        else
        {
            camera.Priority = 9;
            Hexamap.DemoController.instance.BaseOnOff(true);
        }
    }
}
