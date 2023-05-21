using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private Cinemachine.CinemachineVirtualCamera currCam;

    [Serializable]
    public class CameraInfo
    {
        [SerializeField] public Cinemachine.CinemachineVirtualCamera vCam;
        [SerializeField] public ENotePageType notePageType;
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
