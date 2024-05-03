using UnityEngine;
using UnityEngine.Video;

public class DeveloperManager : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    private void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>(); 
        videoPlayer.loopPointReached += OnVideoEnd;

        App.Manager.Sound.PlaySFX("SFX_Logo");
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        App.LoadScene(SceneName.Title);
    }
}
