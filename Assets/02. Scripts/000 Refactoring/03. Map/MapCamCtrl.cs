using System.Collections;
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
    private Transform Player;

    public void Init()
    {
        transposer = mapCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        UI = App.Manager.UI.GetPanel<MapPanel>();
        Map = App.Manager.Map;
        Sound = App.Manager.Sound;
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        mapCamera.Follow = Player.transform;
        mapCamera.m_Lens.OrthographicSize = 6.5f;

        transposer.m_CameraDistance = 5f;
    }

    public void ReInit()
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
            App.Manager.UI.PopUIStack();
        }

        UI.gameObject.SetActive(isOn);
    }

    public void GoToShelter()
    {
        Sound.PlaySFX("SFX_SceneChange_MapToBase");

        Sequence sequence = DOTween.Sequence();
        sequence.Append(DOTween.To(() => transposer.m_CameraDistance, x => transposer.m_CameraDistance = x, 5f, 0.5f))
            .OnComplete(() =>
            {
                SetPrioryty(false);
                Sound.PlayBGM("BGM_InGameTheme");
                UI.FalseTileInfo();
            });
    }

    public void GoToMap()
    {
        App.Manager.Sound.PlaySFX("SFX_SceneChange_BaseToMap");

        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() =>
            {
                SetPrioryty(true);
                var currTile = Map.mapCtrl.Player.TileController.GetComponent<TileBase>();
                Sound.PlayBGM(Map.GetLandformBGM(currTile));
                UI.FalseTileInfo();
            })
            .Append(DOTween.To(() => transposer.m_CameraDistance, x => transposer.m_CameraDistance = x, 10f, 0.5f))
            .OnComplete(()=> 
            {
                App.Manager.Map.GetCameraCenterTile();
                App.Manager.Map.mapCtrl.InvocationExplorers();
            });
    }
}
