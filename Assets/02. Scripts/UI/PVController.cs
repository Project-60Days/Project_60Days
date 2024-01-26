using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using DG.Tweening;

public class PVController : MonoBehaviour
{
    VideoPlayer videoPlayer;
    [SerializeField] GameObject PVImage;
    RawImage pvImage;
    [SerializeField] Image blackPanel;

    [SerializeField] TextMeshProUGUI text;
    [SerializeField] VideoClip PV01;

    [HideInInspector] public bool isEnd = true;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        pvImage = PVImage.GetComponent<RawImage>();

        isEnd = true;

        text.alpha = 0f;
        text.gameObject.SetActive(false);

        pvImage.DOFade(0f, 0f);
        PVImage.SetActive(false);

        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void Update()
    {
        if (isEnd == false && Input.GetKeyDown(KeyCode.P))
        {
            videoPlayer.Stop();
            OnVideoEnd(videoPlayer);
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        isEnd = true;

        pvImage.DOFade(0f, 1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            text.gameObject.SetActive(false);

            blackPanel.DOFade(0f, 0f);
            blackPanel.gameObject.SetActive(false);

            PVImage.SetActive(false);

            UIManager.instance.PopCurrUI();
            App.instance.GetSoundManager().PlayBGM("BGM_InGameTheme");
        });
    }

    public void Start01()
    {
        ReadyToStart();

        videoPlayer.clip = PV01;

        blackPanel.DOFade(1f, 1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            text.DOFade(1f, 0f);

            videoPlayer.Play();
            pvImage.DOFade(1f, 0.5f).SetEase(Ease.Linear);

            FadeOutText();
        });
    }

    void ReadyToStart()
    {
        isEnd = false;

        blackPanel.gameObject.SetActive(true);

        PVImage.SetActive(true);

        UIManager.instance.AddCurrUIName("UI_PV");
        App.instance.GetSoundManager().StopBGM();
    }

    void FadeOutText()
    {
        text.gameObject.SetActive(true);

        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(3f)
                .Append(text.DOFade(0f, 1f).SetEase(Ease.Linear));
    }
}
