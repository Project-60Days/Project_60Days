using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class MapCamCtrl : MonoBehaviour, IListener
{
    [SerializeField] CinemachineVirtualCamera mapCamera;
    private CinemachineFramingTransposer transposer;

    private MapPanel MapUI;
    private MapManager Map;
    private SoundManager Sound;

    private void Awake()
    {
        App.Manager.Event.AddListener(EventCode.PlayerCreate, this);
        App.Manager.Event.AddListener(EventCode.GoToMap, this);
        App.Manager.Event.AddListener(EventCode.GoToShelter, this);
        App.Manager.Event.AddListener(EventCode.NextDayStart, this);
    }

    public void OnEvent(EventCode _code, Component _sender, object _param = null)
    {
        switch (_code)
        {
            case EventCode.PlayerCreate:
                mapCamera.Follow = _param as Transform;
                mapCamera.m_Lens.OrthographicSize = 6.5f;

                transposer = mapCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
                transposer.m_CameraDistance = 5f;
                break;


            case EventCode.GoToMap:
                App.Manager.UI.FadeInOut(GoToMap);
                break;

            case EventCode.GoToShelter:
                App.Manager.UI.FadeInOut();
                GoToShelter();
                break;

            case EventCode.NextDayStart:
                ResetCamera();
                break;
        }
    }

    private void Start()
    {
        MapUI = App.Manager.UI.GetPanel<MapPanel>();
        Map = App.Manager.Map;
        Sound = App.Manager.Sound;
    }

    private void SetPrioryty(bool isOn)
    {
        // 230726 JHJ 임시로 카메라 priority 수정 향후 수정 필요
        if (isOn)
        {
            mapCamera.Priority = 11;
        }
        else
        {
            mapCamera.Priority = 8;
        }

        MapUI.SetActive(isOn);
        Map.isMapActive = isOn;
    }

    private void GoToShelter()
    {
        Sound.PlaySFX("SFX_Map_Close");

        Sequence sequence = DOTween.Sequence();
        sequence.Append(DOTween.To(() => transposer.m_CameraDistance, x => transposer.m_CameraDistance = x, 5f, 0.5f))
            .OnComplete(() =>
            {
                SetPrioryty(false);
                Sound.PlayBGM("BGM_InGame");
            });
    }

    private void GoToMap()
    {
        Sound.PlaySFX("SFX_Map_Open");

        Sequence sequence = DOTween.Sequence();
        sequence
            .AppendCallback(() =>
            {
                SetPrioryty(true);
                Sound.PlayBGM(Map.GetLandformBGM());
            })
            .Append(DOTween.To(() => transposer.m_CameraDistance, x => transposer.m_CameraDistance = x, 10f, 0.5f));
    }

    private void ResetCamera()
    {
        transposer.m_CameraDistance = 5f;
        SetPrioryty(false);
    }
}
