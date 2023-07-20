using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MapCamera : MonoBehaviour
{
    GameObject player;
    GameObject noteUi;
    GameObject hpText;
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
/*        noteUi = GameObject.FindGameObjectWithTag("NoteUi");
        hpText = GameObject.FindGameObjectWithTag("MapUi").transform.Find("Hp_Text").gameObject;*/
        mapCamera.Follow = player.transform;
        mapCamera.m_Lens.OrthographicSize = 6;
    }

    public void SetPrioryty(bool isOn)
    {
        if (isOn)
        {
            mapCamera.Priority = 11;
            MapController.instance.BaseActiveSet(false);
            noteUi.SetActive(false);
            hpText.SetActive(true);
        }
        else
        {
            mapCamera.Priority = 9;
            MapController.instance.BaseActiveSet(true);
            noteUi.SetActive(true);
            hpText.SetActive(false);
        }
    }
}
