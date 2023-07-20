using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MapCamera : MonoBehaviour
{
    GameObject player;
    GameObject noteUi;
    GameObject mapUi;
    public CinemachineVirtualCamera mapCamera;

    private void Start()
    {
        StartCoroutine(GetCamera());
        mapCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public IEnumerator GetCamera()
    {
        yield return new WaitForEndOfFrame();
        player = GameObject.FindGameObjectWithTag("Player");
        mapUi = GameObject.FindGameObjectWithTag("MapUi").transform.GetChild(0).gameObject;
        mapCamera.Follow = player.transform;
        mapCamera.m_Lens.OrthographicSize = 6;
    }

    public void SetPrioryty(bool isOn)
    {
        mapUi.SetActive(isOn);
        MapController.instance.BaseActiveSet(!isOn);
    }
}
