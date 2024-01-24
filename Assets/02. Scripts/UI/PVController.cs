using UnityEngine;
using UnityEngine.Video;
using TMPro;
using DG.Tweening;

public class PVController : MonoBehaviour
{
    VideoPlayer videoPlayer;
    [SerializeField] GameObject PVImage;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] VideoClip PV01;

    [HideInInspector] public bool isEnd = true;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        isEnd = true;
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
        PVImage.SetActive(false);
        UIManager.instance.PopCurrUI();
        App.instance.GetSoundManager().PlayBGM("BGM_InGameTheme");
    }
    
    public void Start01()
    {
        ReadyToStart();

        videoPlayer.clip = PV01;
        videoPlayer.Play();

        FadeOutText();
    }

    void ReadyToStart()
    {
        isEnd = false;
        text.alpha = 1f;

        PVImage.SetActive(true);

        UIManager.instance.AddCurrUIName("UI_PV");
        App.instance.GetSoundManager().StopBGM();
    }

    void FadeOutText()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(3f)
                .Append(text.DOFade(0f, 1f));
    }
}
