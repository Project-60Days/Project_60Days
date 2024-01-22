using System.Collections;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Febucci.UI;

public class NextDayController : MonoBehaviour
{
    [SerializeField] Image blackPanel;
    [SerializeField] GameObject dayCountText;
    [SerializeField] TextMeshProUGUI durabillityText;
    TextMeshProUGUI dayCountTextTmp;
    TypewriterByCharacter textAnimatorPlayer;

    CinemachineVirtualCamera mapCamera;
    CameraShake normalCamera;
    CinemachineFramingTransposer transposer;

    [HideInInspector] public bool isOver = false;
    public bool isHit = false;

    void Start()
    {
        mapCamera = GameObject.FindGameObjectWithTag("MapCamera").GetComponent<CinemachineVirtualCamera>();
        normalCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>();

        transposer = mapCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        transposer.m_CameraDistance = 5f;

        dayCountTextTmp = dayCountText.GetComponent<TextMeshProUGUI>();
        textAnimatorPlayer = dayCountText.GetComponent<TypewriterByCharacter>();

        isOver = false;
    }

    public void InitBlackPanel()
    {
        if (isHit == true) normalCamera.Shake(durabillityText);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(blackPanel.DOFade(0f, 2f))
            .OnComplete(() => blackPanel.gameObject.SetActive(false));

        dayCountText.SetActive(false);
        isHit = false;

        UIManager.instance.PopCurrUI();

        UIManager.instance.GetNoteController().isNewDay = true;
    }


    /// <summary>
    /// 다음 날로 넘어갈 때 호출되는 이벤트 --> blackPanel 깜빡거림
    /// </summary>
    public void NextDayEvent()
    {
        UIManager.instance.AddCurrUIName("UI_LOADING");

        blackPanel.gameObject.SetActive(true);

        App.instance.GetSoundManager().StopBGM();

        Sequence sequence = DOTween.Sequence();
        sequence.Append(blackPanel.DOFade(1f, 0.5f)).SetEase(Ease.InQuint)
            .OnComplete(() =>
            {
                StartCoroutine(NextDayEventCallBack(() =>
                {
                    if (isOver == true)
                        StartCoroutine(ShowGameOver());
                    else
                        StartCoroutine(ShowNextDate());
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

        yield return new WaitForSeconds(1f);

        SetResourcesResultPage();

        callback?.Invoke();
    }

    IEnumerator ShowNextDate()
    {
        UIManager.instance.GetNoteController().SetNextDay();
        //UIManager.instance.GetUpperController().UpdateAfterFight();

        int today = UIManager.instance.GetNoteController().dayCount;

        string text = "Day " + "{vertexp}" + today.ToString() + "{/vertexp}";
        dayCountTextTmp.color = Color.white;

        if (isHit == true)
        {
            text = "<shake>" + "Day " + "{vertexp}" + today.ToString() + "{/vertexp}</shake>";
            dayCountTextTmp.color = Color.red;
        }

        textAnimatorPlayer.ShowText(text);
        dayCountText.SetActive(true);

        yield return new WaitForSeconds(2f);

        InitBlackPanel();

        App.instance.GetSoundManager().PlayBGM("BGM_InGameTheme");
    }


    IEnumerator ShowGameOver()
    {
        dayCountText.SetActive(true);
        string text = "<shake>GAME OVER</>";
        dayCountTextTmp.color = Color.red;
        textAnimatorPlayer.ShowText(text);

        yield return new WaitForSeconds(2f);

        GameManager.instance.QuitGame();
    }


    void SetResourcesResultPage()
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
            .OnComplete(() =>
                {
                    blackPanel.gameObject.SetActive(false);
                    App.instance.GetMapManager().GetCameraCenterTile();
                }
            );
    }
}