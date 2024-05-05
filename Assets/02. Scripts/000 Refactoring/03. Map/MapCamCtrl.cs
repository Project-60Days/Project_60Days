using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class MapCamCtrl : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera mapCamera;
    private CinemachineFramingTransposer transposer;

    private MapPanel UI;
    private MapManager Map;
    private SoundManager Sound;

    public void Init()
    {
        UI = App.Manager.UI.GetPanel<MapPanel>();
        Map = App.Manager.Map;
        Sound = App.Manager.Sound;

        Transform player = Map.mapCtrl.playerCtrl.player.GetComponent<Transform>();
        mapCamera.Follow = player;
        mapCamera.m_Lens.OrthographicSize = 6.5f;

        transposer = mapCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        transposer.m_CameraDistance = 5f;
    }

    public void ResetCamera()
    {
        transposer.m_CameraDistance = 5f;
        SetPrioryty(false);
    }

    private void SetPrioryty(bool isOn)
    {
        // 230726 JHJ 임시로 카메라 priority 수정 향후 수정 필요
        if (isOn)
        {
            mapCamera.Priority = 11;
            App.Manager.UI.AddUIStack(UIState.Map);
        }
        else
        {
            mapCamera.Priority = 8;
            App.Manager.UI.PopUIStack(UIState.Map);
        }

        UI.gameObject.SetActive(isOn);
    }

    public void GoToShelter()
    {
        Sound.PlaySFX("SFX_Map_Close");

        Sequence sequence = DOTween.Sequence();
        sequence.Append(DOTween.To(() => transposer.m_CameraDistance, x => transposer.m_CameraDistance = x, 5f, 0.5f))
            .OnComplete(() =>
            {
                SetPrioryty(false);
                Sound.PlayBGM("BGM_InGame");
                UI.TileInfo(false);
            });
    }

    public void GoToMap()
    {
        App.Manager.Sound.PlaySFX("SFX_Map_Open");

        Sequence sequence = DOTween.Sequence();
        sequence
            .AppendCallback(() =>
            {
                SetPrioryty(true);
                var currTile = Map.mapCtrl.tileCtrl.GetComponent<TileBase>();
                Sound.PlayBGM(Map.GetLandformBGM(currTile));
                UI.TileInfo(false);
            })
            .Append(DOTween.To(() => transposer.m_CameraDistance, x => transposer.m_CameraDistance = x, 10f, 0.5f))
            .OnComplete(()=> 
            {
                App.Manager.Map.GetCameraCenterTile();
                App.Manager.Map.mapCtrl.droneCtrl.InvocationExplorers();
            });
    }
}
