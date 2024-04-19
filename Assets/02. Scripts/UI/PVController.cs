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
    bool isPlaying = false;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        pvImage = PVImage.GetComponent<RawImage>();

        isEnd = true;
        isPlaying = false;

        text.gameObject.SetActive(false);

        PVImage.SetActive(false);

        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void Update()
    {
        if (isPlaying == true)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                videoPlayer.Stop();
                OnVideoEnd(videoPlayer);
            }
            else if (Input.anyKeyDown && text.alpha == 0)
            {
                FadeOutText();
            }
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        isPlaying = false;

        text.gameObject.SetActive(false);

        pvImage.DOFade(0f, 1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            PVImage.SetActive(false);

            UIManager.instance.PopCurrUI();
            App.Manager.Sound.PlayBGM("BGM_InGameTheme");

            isEnd = true;
        });
    }

    public void Start01()
    {
        ReadyToStart();

        videoPlayer.clip = PV01;

        blackPanel.DOFade(1f, 1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            PVImage.SetActive(true);

            blackPanel.DOFade(0f, 0f).SetDelay(1f).OnComplete(()=> blackPanel.gameObject.SetActive(false));
            
            videoPlayer.Play();

            FadeOutText();

            isPlaying = true;
        });
    }

    void ReadyToStart()
    {
        isEnd = false;

        blackPanel.gameObject.SetActive(true);

        UIManager.instance.AddCurrUIName("UI_PV");
        App.Manager.Sound.StopBGM();
    }

    void FadeOutText()
    {
        text.gameObject.SetActive(true);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(text.DOFade(1f, 0f))
                .AppendInterval(3f)
                .Append(text.DOFade(0f, 1f).SetEase(Ease.Linear));
    }
}
