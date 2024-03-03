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

        UIManager.instance.GetUpperController().DecreaseDurabillityAnimation();

        mainCamera.transform.DOShakePosition(1f);
    }

    void PlaySFX()
    {
        int sfxIndex = Random.Range(1, 5);
        App.instance.GetSoundManager().StopSFX();
        App.instance.GetSoundManager().PlaySFX("SFX_HIT_" + sfxIndex.ToString());
    }
}