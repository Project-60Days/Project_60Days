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

    public IEnumerator GetMapInfo()
    {
        yield return new WaitForEndOfFrame();
        player = GameObject.FindGameObjectWithTag("Player");
        mapUi = GameObject.FindGameObjectWithTag("MapUi").transform.GetChild(0).gameObject;
        mapCamera.Follow = player.transform;
        mapCamera.m_Lens.OrthographicSize = 6.5f;
    }

    public void SetPrioryty(bool isOn)
    {
        // 230726 JHJ 임시로 카메라 priority 수정 향후 수정 필요
        if (isOn)
        {
            mapCamera.Priority = 11;
            App.Manager.UI.AddCurrUIName(UIState.Map);
        }
        else
        {
            mapCamera.Priority = 8;
            App.Manager.UI.PopCurrUI();
        }
        mapUi.SetActive(isOn);
    }
}
