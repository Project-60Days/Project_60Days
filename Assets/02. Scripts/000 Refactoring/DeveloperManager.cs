using UnityEngine;
using UnityEngine.Video;

public class DeveloperManager : MonoBehaviour
{
    VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>(); 
        videoPlayer.loopPointReached += OnVideoEnd;

        App.instance.GetSoundManager().PlaySFX("SFX_Title_LogIn");
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        App.LoadScene(ESceneType.Title);
    }
}
