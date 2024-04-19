using UnityEngine;
using TMPro;
using Cinemachine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera mainCamera;

    public void Shake()
    {
        PlaySFX();

        App.Manager.UI.GetUpperController().DecreaseDurabillityAnimation();

        mainCamera.transform.DOShakePosition(1f);
    }

    void PlaySFX()
    {
        int sfxIndex = Random.Range(1, 5);
        App.Manager.Sound.StopSFX();
        App.Manager.Sound.PlaySFX("SFX_HIT_" + sfxIndex.ToString());
    }
}