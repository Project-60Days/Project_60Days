using System.Collections;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class NextDayController : ControllerBase
{
    [SerializeField] Image blackPanel;

    CinemachineVirtualCamera mapCamera;
    CinemachineFramingTransposer transposer;

    public override EControllerType GetControllerType()
    {
        return EControllerType.NEXTDAY;
    }

    void Start()
    {
        mapCamera = GameObject.FindGameObjectWithTag("MapCamera").GetComponent<CinemachineVirtualCamera>();
        transposer = mapCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        transposer.m_CameraDistance = 5f;
        App.instance.AddController(this); //?
    }

    public void InitBlackPanel()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(blackPanel.DOFade(0f, 1f))
            .OnComplete(() => blackPanel.gameObject.SetActive(false));
    }

    /// <summary>
    /// 다음 날로 넘어갈 때 호출되는 이벤트 --> blackPanel 깜빡거림
    /// </summary>
    public void NextDayEvent()
    {
        blackPanel.gameObject.SetActive(true);

        App.instance.GetSoundManager().StopBGM();

        Sequence sequence = DOTween.Sequence();
        sequence.Append(blackPanel.DOFade(1f, 0.5f)).SetEase(Ease.InQuint)
            .OnComplete(() => {
                StartCoroutine(NextDayEventCallBack(()=> {
                    InitBlackPanel();
                    UIManager.instance.GetNoteController().SetNextDay();
                    App.instance.GetSoundManager().PlayBGM("BGM_InGameTheme");
                }));
            });
    }

    /// <summary>
    /// NextDayEvent 콜백함수
    /// </summary>
    IEnumerator NextDayEventCallBack(System.Action callback)
    {
        UIManager.instance.GetAlertController().InitAlert();

        App.instance.GetMapManager().SetMapCameraPriority(false);
        transposer.m_CameraDistance = 5f;

        yield return StartCoroutine(App.instance.GetMapManager().NextDayCoroutine());

        UIManager.instance.GetNextDayController().SetResourcesResultPage();

        callback?.Invoke();
    }

    public void SetResourcesResultPage()
    {
        var resources = App.instance.GetMapManager().resourceManager.GetLastResources();

        for (int i = 0; i < resources.Count; i++)
        {
            string tileName = App.instance.GetMapManager().mapController
                .Player.TileController.GetComponent<TileBase>().TileData.English;

            int randomNumber = Random.Range(1, 6);

            string nodeName = resources[i].ItemBase.data.Code + "_" + tileName + randomNumber.ToString();

            UIManager.instance.GetPageController().SetResultPage(nodeName, true);
        }
    }

    /// <summary>
    /// 맵->기지 카메라 이동
    /// </summary>
    public void GoToLab()
    {
        blackPanel.gameObject.SetActive(true);

        App.instance.GetSoundManager().StopBGM();
        App.instance.GetSoundManager().PlaySFX("SFX_SceneChange_MapToBase");
        
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(blackPanel.DOFade(1f, 0.5f))
            .Join(DOTween.To(() => transposer.m_CameraDistance, x => transposer.m_CameraDistance = x, 5f, 0.5f))
            .AppendCallback(() =>
            {
                App.instance.GetMapManager().SetMapCameraPriority(false);
                App.instance.GetSoundManager().PlayBGM("BGM_InGameTheme");
            })
            .Append(blackPanel.DOFade(0f, 1f))
            .OnComplete(() => blackPanel.gameObject.SetActive(false));
    }

    /// <summary>
    /// 기지->맵 카메라 이동
    /// </summary>
    public void GoToMap()
    {
        blackPanel.gameObject.SetActive(true);

        App.instance.GetSoundManager().StopBGM();

        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(blackPanel.DOFade(1f, 0.5f))
            .AppendCallback(() =>
            {
                App.instance.GetSoundManager().PlaySFX("SFX_SceneChange_BaseToMap");

                App.instance.GetMapManager().SetMapCameraPriority(true);
                App.instance.GetMapManager().CheckLandformPlayMusic();
            })
            .Append(DOTween.To(() => transposer.m_CameraDistance, x => transposer.m_CameraDistance = x, 10f, 0.5f))
            .Join(blackPanel.DOFade(0f, 0.5f))
            .OnComplete(() => blackPanel.gameObject.SetActive(false));
    }
}