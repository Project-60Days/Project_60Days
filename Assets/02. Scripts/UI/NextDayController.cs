using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class NextDayController : ControllerBase
{
    [SerializeField] Image blackPanel;

    [Header("Quest Objects")]
    [SerializeField] GameObject questPrefab;
    [SerializeField] Transform questParent;
    [SerializeField] GameObject questLogo;

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

    #region Inits
    /// <summary>
    /// BlackPanel 초기화 --> BlackPanel: 다음 날로 넘어갈 때 깜빡거리는 용도
    /// </summary>
    void InitBlackPanel()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(blackPanel.DOFade(0f, 1f))
            .OnComplete(() => blackPanel.gameObject.SetActive(false));
        sequence.Play();
    }    
    #endregion





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
        sequence.Play();
    }

    /// <summary>
    /// NextDayEvent 콜백함수
    /// </summary>
    void NextDayEventCallBack()
    {
        Init();

        App.instance.GetMapManager().SetMapCameraPriority(false);
        transposer.m_CameraDistance = 15f;

        UIManager.instance.GetNoteController().SetNextDay();

        App.instance.GetMapManager().AllowMouseEvent(true);

        StartCoroutine(App.instance.GetMapManager().NextDayCoroutine());
    }

    /// <summary>
    /// 맵->기지 카메라 이동
    /// </summary>
    public void GoToLab()
    {
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(DOTween.To(() => transposer.m_CameraDistance, x => transposer.m_CameraDistance = x, 5f, 0.5f))
            .OnComplete(() => App.instance.GetMapManager().SetMapCameraPriority(false))
            .Append(shelterUi.DOFade(1f, 0.5f));
        sequence.Play();
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
        sequence.Play();
    }

    /// <summary>
    /// GoToMap()에서 호출하는 콜백함수
    /// </summary>
    void ZoomInMap()
    {
        App.instance.GetMapManager().SetMapCameraPriority(true);
        blackPanel.DOFade(0f, 0.5f);
        DOTween.To(() => transposer.m_CameraDistance, x => transposer.m_CameraDistance = x, 10f, 0.5f)
            .OnComplete(() => blackPanel.gameObject.SetActive(false));
    }
}