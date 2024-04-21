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
    [SerializeField] GameObject dayCountPrefab;
    GameObject dayCount;
    [SerializeField] MapIcon mapIcon;

    CinemachineVirtualCamera mapCamera;
    CinemachineFramingTransposer transposer;

    [HideInInspector] public bool isOver = false;
    [HideInInspector] public bool isHit = false;

    void Start()
    {
        mapCamera = GameObject.FindGameObjectWithTag("MapCamera").GetComponent<CinemachineVirtualCamera>();

        transposer = mapCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        transposer.m_CameraDistance = 5f;

        isOver = false;
    }

    public void InitBlackPanel()
    {
        if (isHit == true)
        {
            App.Manager.Shelter.Attack();
        }
        else
            App.Manager.UI.GetUpperController().UpdateDurabillity();

        App.Manager.UI.FadeOut();

        Destroy(dayCount);

        isHit = false;

        App.Manager.UI.PopUIStack();
        //todo

        App.Manager.UI.GetPanel<NotePanel>().isNewDay = true;
    }


    /// <summary>
    /// 다음 날로 넘어갈 때 호출되는 이벤트 --> blackPanel 깜빡거림
    /// </summary>
    public void NextDayEvent()
    {
        App.Manager.UI.AddUIStack(UIState.EndDay);

        blackPanel.gameObject.SetActive(true);

        App.Manager.Sound.StopBGM();

        App.Manager.UI.FadeIn(EndFadeIn);
    }
    
    void EndFadeIn()
    {
        StartCoroutine(NextDayEventCallBack(() =>
        {
            if (isOver == true)
                StartCoroutine(ShowGameOver());
            else
                StartCoroutine(ShowNextDate());
        }));
    }

    /// <summary>
    /// NextDayEvent 콜백함수
    /// </summary>
    IEnumerator NextDayEventCallBack(System.Action callback)
    {
        App.Manager.Map.SetMapCameraPriority(false);
        transposer.m_CameraDistance = 5f;

        yield return StartCoroutine(App.Manager.Map.NextDayCoroutine());

        yield return new WaitForSeconds(1f);

        SetResourcesResultPage();

        callback?.Invoke();
    }

    IEnumerator ShowNextDate()
    {
        App.Manager.UI.GetPanel<NotePanel>().ReInit();
        App.Manager.UI.GetPanel<CraftPanel>().Equip.EquipItemDayEvent();

        int today = App.Manager.UI.GetPanel<NotePanel>().dayCount;

        string text = "<color=white>Day " + "{vertexp}" + today.ToString() + "{/vertexp}</color>";

        if (isHit == true)
            text = "<color=red><shake a=0.1>" + "Day " + "{vertexp}" + today.ToString() + "{/vertexp}</shake></color>";

        CreateDayCountTxt(text);
        mapIcon.SetIconImage();

        yield return new WaitForSeconds(2f);

        InitBlackPanel();

        App.Manager.Sound.PlayBGM("BGM_InGameTheme");
    }


    IEnumerator ShowGameOver()
    {
        string text = "<color=red><shake a=0.1>GAME OVER</shake></color>";
        CreateDayCountTxt(text);

        yield return new WaitForSeconds(2f);

        Application.Quit();
    }

    void CreateDayCountTxt(string _text)
    {
        dayCount = Instantiate(dayCountPrefab, blackPanel.transform);
        TextMeshProUGUI text = dayCount.GetComponent<TextMeshProUGUI>();
        text.text = _text;
    }


    void SetResourcesResultPage()
    {
        var resources = App.Manager.Map.resourceManager.GetLastResources();

        for (int i = 0; i < resources.Count; i++)
        {
            string tileName = App.Manager.Map.mapController
                .Player.TileController.GetComponent<TileBase>().TileData.English;

            int randomNumber = Random.Range(1, 6);

            string nodeName = resources[i].ItemBase.data.Code + "_" + tileName + randomNumber.ToString();

            if (resources[i].ItemBase.data.Code == "ITEM_NETWORKCHIP")
                App.Manager.UI.GetPageController().SetResultPage(nodeName, false);
            else
                App.Manager.UI.GetPageController().SetResultPage(nodeName, true);
        }
    }

    /// <summary>
    /// 맵->기지 카메라 이동
    /// </summary>
    public void GoToLab()
    {
        blackPanel.gameObject.SetActive(true);

        App.Manager.Sound.StopBGM();
        App.Manager.Sound.PlaySFX("SFX_SceneChange_MapToBase");

        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(blackPanel.DOFade(1f, 0.5f))
            .Join(DOTween.To(() => transposer.m_CameraDistance, x => transposer.m_CameraDistance = x, 5f, 0.5f))
            .AppendCallback(() =>
            {
                App.Manager.Map.SetMapCameraPriority(false);
                App.Manager.Sound.PlayBGM("BGM_InGameTheme");
                App.Manager.Map.mapUIController.FalseTileInfo();
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

        App.Manager.Sound.StopBGM();

        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(blackPanel.DOFade(1f, 0.5f))
            .AppendCallback(() =>
            {
                App.Manager.Map.mapUIController.FalseTileInfo();
                App.Manager.Sound.PlaySFX("SFX_SceneChange_BaseToMap");

                App.Manager.Map.SetMapCameraPriority(true);
                App.Manager.Map.CheckLandformPlayMusic();
            })
            .Append(DOTween.To(() => transposer.m_CameraDistance, x => transposer.m_CameraDistance = x, 10f, 0.5f))
            .Join(blackPanel.DOFade(0f, 0.5f))
            .OnComplete(() =>
                {
                    blackPanel.gameObject.SetActive(false);
                    App.Manager.Map.GetCameraCenterTile();
                    App.Manager.Map.InvocationExplorers();
                }
            );
    }
}