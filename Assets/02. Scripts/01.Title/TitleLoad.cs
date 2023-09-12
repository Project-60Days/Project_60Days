using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using DG.Tweening;
using TMPro;

public class TitleLoad : MonoBehaviour
{
    [Header("Loading Objects")]
    [SerializeField] GameObject loadingVideo;
    VideoPlayer videoPlayer;

    [SerializeField] TextMeshProUGUI leftLogField;
    string leftFileText;
    [SerializeField] TextMeshProUGUI rightLogField;
    string rightFileText;
    string[] lines;

    [SerializeField] ScrollRect rightLogScrollRect;

    [SerializeField] float leftLogShowInterval;
    [SerializeField] float rightLogShowInterval;

    [Header("Title Objects")]
    [SerializeField] GameObject titleText;
    [SerializeField] GameObject titleImage;

    [SerializeField] GameObject buttonText;
    [SerializeField] GameObject buttonBack;





    void Start()
    {
        Init();
    }

    void Init()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += OnVideoEnd;

        string leftfilePath = "Assets/Text/Tittle_Log_LeftAccess.txt";
        leftFileText = AssetDatabase.LoadAssetAtPath<TextAsset>(leftfilePath).text;

        string rightfilePath = "Assets/Text/Tittle_Log_RightLog.txt";
        rightFileText = AssetDatabase.LoadAssetAtPath<TextAsset>(rightfilePath).text;

        lines = rightFileText.Split('\n');

        InitObjects();
    }

    void InitObjects()
    {
        ActiveTitleObjects(false);
        ActiveButtonObjects(false);
    }





    void ActiveTitleObjects(bool isActive)
    {
        titleText.SetActive(isActive);
        titleImage.SetActive(isActive);
    }

    void ActiveButtonObjects(bool isActive)
    {
        buttonText.SetActive(isActive);
        buttonBack.SetActive(isActive);
    }





    /// <summary>
    /// 비디오 재생이 끝났을 때 비활성화하는 함수
    /// </summary>
    /// <param name="vp"></param>
    void OnVideoEnd(VideoPlayer vp)
    {
        loadingVideo.SetActive(false);
        StartCoroutine(LeftLog());
    }





    /// <summary>
    /// 좌측상단 로그 재생
    /// </summary>
    /// <returns></returns>
    IEnumerator LeftLog()
    {
        int currentIndex = 0;
        leftLogField.text = "";

        while (currentIndex < leftFileText.Length)
        {
            char currentChar = leftFileText[currentIndex];
            leftLogField.text += currentChar;

            if (currentChar != '=')
            {
                yield return new WaitForSeconds(leftLogShowInterval);
            }

            currentIndex++;
        }

        StartCoroutine(RightLog());
    }

    /// <summary>
    /// 우측상단 로그 재생
    /// </summary>
    /// <returns></returns>
    IEnumerator RightLog()
    {
        int currentIndex = 0;
        rightLogField.text = "";

        while (currentIndex < lines.Length)
        {
            string line = lines[currentIndex];
            rightLogField.text += line + '\n';
            currentIndex++;

            rightLogScrollRect.verticalNormalizedPosition = 0.0f;

            yield return new WaitForSeconds(rightLogShowInterval);
        }

        StartCoroutine(Title());
    }





    /// <summary>
    /// 타이틀 재생
    /// </summary>
    /// <returns></returns>
    IEnumerator Title()
    {
        yield return new WaitForSeconds(2f);

        ActiveTitleObjects(true);

        SetTitleText();
        SetTitleImage();

        App.instance.GetSoundManager().PlayBGM("BGM_TitleTheme");

        yield return new WaitForSeconds(2f);

        ActiveButtonObjects(true);
    }

    void SetTitleText()
    {
        TextMeshProUGUI text = titleText.GetComponent<TextMeshProUGUI>();

        text.alpha = 0f;
        text.DOFade(1f, 0.1f).SetEase(Ease.InOutBounce);
    }

    void SetTitleImage()
    {
        Image title = titleImage.GetComponent<Image>();

        title.color = new Color(1f, 1f, 1f, 0f);
        title.DOFade(1f, 0.1f).SetEase(Ease.InOutBounce);
    }
}
