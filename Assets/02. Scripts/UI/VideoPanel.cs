using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using DG.Tweening;

public class VideoPanel : UIBase
{
    [SerializeField] VideoPlayer videoPlayer;
    RawImage pvImage;

    [SerializeField] TextMeshProUGUI text;
    [SerializeField] VideoClip PV01;

    [HideInInspector] public bool isEnd = true;
    bool isPlaying = false;

    #region Override
    public override void Init()
    {
        pvImage = GetComponent<RawImage>();
        pvImage.DOFade(0f, 0f);

        isEnd = true;
        isPlaying = false;

        text.gameObject.SetActive(false);

        videoPlayer.loopPointReached += OnVideoEnd;

        gameObject.SetActive(false);
    }

    public override UIState GetUIState() => UIState.PV;

    public override bool IsAddUIStack() => true;

    public override void OpenPanel()
    {
        base.OpenPanel();

        isEnd = false;
    }

    public override void ClosePanel()
    {
        base.ClosePanel();

        isEnd = true;

        App.Manager.Sound.PlayBGM("BGM_InGame");
    }
    #endregion

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

    public void Start01()
    {
        OpenPanel();

        videoPlayer.clip = PV01;

        App.Manager.UI.FadeIn(Play01);
    }

    public void Play01()
    {
        pvImage.DOFade(1f, 0f);

        videoPlayer.Play();

        FadeOutText();

        isPlaying = true;
    }

    void FadeOutText()
    {
        text.gameObject.SetActive(true);

        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(3f)
                .Append(text.DOFade(1f, 1f).SetEase(Ease.Linear).From());
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        isPlaying = false;

        text.gameObject.SetActive(false);

        pvImage.DOFade(0f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            App.Manager.UI.FadeOut(ClosePanel);
        });
    }
}
