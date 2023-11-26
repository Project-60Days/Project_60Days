using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class NextDayController : ControllerBase
{
    [SerializeField] Image blackPanel;

    CanvasGroup shelterUi;

    CinemachineVirtualCamera mapCamera;
    CinemachineFramingTransposer transposer;

    public override EControllerType GetControllerType()
    {
        return EControllerType.NEXTDAY;
    }

    void Awake()
    {
        Init();
    }

    void Start()
    {
        shelterUi = GameObject.FindGameObjectWithTag("ShelterUi").GetComponent<CanvasGroup>();
        mapCamera = GameObject.FindGameObjectWithTag("MapCamera").GetComponent<CinemachineVirtualCamera>();
        transposer = mapCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        transposer.m_CameraDistance = 5f;
        App.instance.AddController(this); //?
    }

    /// <summary>
    /// 초기화 함수 모음
    /// </summary>
    void Init()
    {
        InitBlackPanel();
        UIManager.instance.GetAlertController().InitAlert();
    }

    void InitBlackPanel()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(blackPanel.DOFade(0f, 1f))
            .OnComplete(() => blackPanel.gameObject.SetActive(false));
        sequence.Play();
    }

    /// <summary>
    /// 다음 날로 넘어갈 때 호출되는 이벤트 --> blackPanel 깜빡거림
    /// </summary>
    public void NextDayEvent()
    {
        blackPanel.gameObject.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(blackPanel.DOFade(1f, 0.5f)).SetEase(Ease.InQuint)
            .AppendInterval(0.5f)
            .Append(shelterUi.DOFade(1f, 0f))
            .OnComplete(() => NextDayEventCallBack());
    }

    /// <summary>
    /// NextDayEvent 콜백함수
    /// </summary>
    void NextDayEventCallBack()
    {
        App.instance.GetSoundManager().PlayBGM("BGM_InGameTheme");
        Init();

        App.instance.GetMapManager().SetMapCameraPriority(false);
        transposer.m_CameraDistance = 15f;

        SetResourcesResultPage();
        UIManager.instance.GetNoteController().SetNextDay();

        StartCoroutine(App.instance.GetMapManager().NextDayCoroutine());
    }

    void SetResourcesResultPage()
    {
        var resources = App.instance.GetMapManager().resourceManager.GetLastResources();

        for (int i = 0; i < resources.Count; i++)
        {
            string tileName = App.instance.GetMapManager().mapController
                .Player.TileController.GetComponent<TileBase>().TileData.English;
            string nodeName = resources[i].ItemBase.data.Code + "_" + tileName + "1" + ToString();

            Debug.Log(nodeName);
            UIManager.instance.GetPageController().SetResultPage(nodeName);
        }
    }

    /// <summary>
    /// 맵->기지 카메라 이동
    /// </summary>
    public void GoToLab()
    {
        App.instance.GetSoundManager().PlayBGM("BGM_InGameTheme");
        App.instance.GetSoundManager().PlaySFX("SFX_SceneChange_MapToBase");
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(DOTween.To(() => transposer.m_CameraDistance, x => transposer.m_CameraDistance = x, 5f, 0.5f))
            .OnComplete(() =>
            {
                App.instance.GetMapManager().SetMapCameraPriority(false);

            })
            .Append(shelterUi.DOFade(1f, 0.5f));
    }

    /// <summary>
    /// 기지->맵 카메라 이동
    /// </summary>
    public void GoToMap()
    {
        blackPanel.gameObject.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(shelterUi.DOFade(0f, 0.5f))
            .Join(blackPanel.DOFade(1f, 0.5f))
            .OnComplete(() => ZoomInMap());
    }

    /// <summary>
    /// GoToMap()에서 호출하는 콜백함수
    /// </summary>
    void ZoomInMap()
    {
        App.instance.GetSoundManager().PlaySFX("SFX_SceneChange_BaseToMap");
        App.instance.GetMapManager().SetMapCameraPriority(true);
        blackPanel.DOFade(0f, 0.5f);
        DOTween.To(() => transposer.m_CameraDistance, x => transposer.m_CameraDistance = x, 10f, 0.5f)
            .OnComplete(() =>
            {
                blackPanel.gameObject.SetActive(false);

                App.instance.GetMapManager().CheckLandformPlayMusic();
            });
    }
}