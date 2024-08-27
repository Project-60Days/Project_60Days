using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using DG.Tweening;

public class VideoPanel : UIBase
{
    [Serializable]
    public class VideoData
    {
        public string Code;
        public VideoClip Clip;
    }

    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] List<VideoData> videoList;

    private RawImage pvImage;
    private Dictionary<string, VideoClip> videoDic;

    public bool IsEnd { get; private set; } = true;
    private bool isPlaying = false;

    private Sequence sequence;

    #region Override
    public override void Init()
    {
        videoDic = new(videoList.Count);

        foreach (var video in videoList)
        {
            videoDic[video.Code] = video.Clip;
        }

        videoList.Clear();

        pvImage = GetComponent<RawImage>();

        IsEnd = true;
        isPlaying = false;

        videoPlayer.loopPointReached += OnVideoEnd;

        gameObject.SetActive(false);
    }

    public override UIState GetUIState() => UIState.PV;

    public override bool IsAddUIStack() => true;

    public override void OpenPanel()
    {
        base.OpenPanel();

        IsEnd = false;
    }

    public override void ClosePanel()
    {
        base.ClosePanel();

        IsEnd = true;

        App.Manager.Sound.PlayBGM("BGM_InGame");
    }
    #endregion

    private void Update()
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
                FadeInText();
            }
        }
    }

    public void StartVideo(string _code)
    {
        OpenPanel();

        videoPlayer.clip = videoDic[_code];

        App.Manager.UI.FadeIn(PlayVideo);
    }

    private void PlayVideo()
    {
        pvImage.DOFade(1f, 0f);

        videoPlayer.Play();

        FadeInText();

        isPlaying = true;
    }

    private void FadeInText()
    {
        sequence.Kill();

        sequence = DOTween.Sequence();
        sequence.AppendInterval(3f)
                .Append(text.DOFade(1f, 1f).SetEase(Ease.Linear).From());
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        isPlaying = false;

        text.gameObject.SetActive(false);

        pvImage.DOFade(0f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            App.Manager.UI.FadeOut(ClosePanel);
        });
    }
}
