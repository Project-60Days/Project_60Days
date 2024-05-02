using UnityEngine;
using UnityEngine.Video;

public class DeveloperManager : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    private void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>(); 
        videoPlayer.loopPointReached += OnVideoEnd;

        App.Manager.Sound.PlaySFX("SFX_Title_LogIn");
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        App.LoadScene(SceneName.Title);
    }
}
