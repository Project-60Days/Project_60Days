using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class MainCamCtrl : MonoBehaviour, IListener
{
    [SerializeField] CinemachineVirtualCamera mainCamera;

    private void Awake()
    {
        App.Manager.Event.AddListener(EventCode.Hit, this);
    }

    public void OnEvent(EventCode _code, Component _sender, object _param = null)
    {
        switch (_code)
        {
            case EventCode.Hit:
                Shake();
                break;
        }
    }

    private void Shake()
    {
        PlaySFX();

        mainCamera.transform.DOShakePosition(1f);
    }

    private void PlaySFX()
    {
        int sfxIndex = Random.Range(1, 5);
        App.Manager.Sound.StopSFX();
        App.Manager.Sound.PlaySFX("SFX_Hit_" + sfxIndex.ToString());
    }
}