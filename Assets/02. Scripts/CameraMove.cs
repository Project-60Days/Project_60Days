using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private Cinemachine.CinemachineVirtualCamera currCam;
    [SerializeField] private Cinemachine.CinemachineBrain brain;

    [Serializable]
    public class CameraInfo
    {
        public CameraInfo(Cinemachine.CinemachineVirtualCamera vCam, ENotePageType notePageType)
        {
            this.vCam = vCam;
            this.notePageType = notePageType;
        }

        [SerializeField] public Cinemachine.CinemachineVirtualCamera vCam;
        [SerializeField] public ENotePageType notePageType;
    }

    private void Start()
    {
        StartCoroutine(GetMapCamera());
    }

    private IEnumerator GetMapCamera()
    {
        yield return new WaitForSeconds(1f);
        cameraInfos.Add(new CameraInfo(GameObject.FindGameObjectWithTag("MapCamera").GetComponent<MapCamera>().mapCamera, ENotePageType.Map));
        ChangeCamera(ENotePageType.DayStart);
    }

    [SerializeField] public List<CameraInfo> cameraInfos;

    public void ChangeCamera(ENotePageType _notePageType)
    {
        var info = cameraInfos.Find(x => x.notePageType == _notePageType);

        if(info.vCam != currCam)
        {
            currCam.Priority --;
            currCam = info.vCam;
            currCam.Priority ++;
        }
    }
}
