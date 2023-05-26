using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MapCamera : MonoBehaviour
{
    GameObject player;
    CinemachineVirtualCamera mapCamera;
    GameObject noteUi;
    GameObject hpText;

    private void Start()
    {
        StartCoroutine(GetCamera());
        mapCamera = gameObject.GetComponent<CinemachineVirtualCamera>();
    }

    public IEnumerator GetCamera()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        player = GameObject.FindGameObjectWithTag("Player");
        noteUi = GameObject.FindGameObjectWithTag("NoteUi");
        hpText = GameObject.FindGameObjectWithTag("MapUi").transform.Find("Hp_Text").gameObject;
        mapCamera.Follow = player.transform;
    }

    public void SetPrioryty(bool isOn)
    {
        if (isOn)
        {
            mapCamera.Priority = 11;
            Hexamap.DemoController.instance.BaseOnOff(false);
            noteUi.SetActive(false);
            hpText.SetActive(true);
        }
        else
        {
            mapCamera.Priority = 9;
            Hexamap.DemoController.instance.BaseOnOff(true);
            noteUi.SetActive(true);
            hpText.SetActive(false);
        }
    }
}
