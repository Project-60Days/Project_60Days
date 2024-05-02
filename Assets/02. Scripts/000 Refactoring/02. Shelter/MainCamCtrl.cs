using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class MainCamCtrl : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera mainCamera;

    public void Shake()
    {
        PlaySFX();

        App.Manager.UI.GetPanel<UpperPanel>().DecreaseDurabillityAnimation();

        mainCamera.transform.DOShakePosition(1f);
    }

    private void PlaySFX()
    {
        int sfxIndex = Random.Range(1, 5);
        App.Manager.Sound.StopSFX();
        App.Manager.Sound.PlaySFX("SFX_HIT_" + sfxIndex.ToString());
    }
}