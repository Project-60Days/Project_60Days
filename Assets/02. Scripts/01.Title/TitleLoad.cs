using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using DG.Tweening;
using TMPro;

public class TitleLoad : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] GameObject rawImage;
    VideoPlayer videoPlayer;

    [SerializeField] TextMeshProUGUI leftLogField;
    string leftFileText;
    [SerializeField] TextMeshProUGUI rightLogField;
    string rightFileText;

    [SerializeField] ScrollRect scrollRect;
    [SerializeField] RectTransform contentRect;

    [SerializeField] float leftLogShowInterval;
    [SerializeField] float rightLogShowInterval;

    [Header("Objects")]
    [SerializeField] GameObject titleText;
    [SerializeField] GameObject titleImage;

    [SerializeField] GameObject buttonText;
    [SerializeField] GameObject buttonBack;

    [SerializeField] GameObject optionPanel;
    [SerializeField] GameObject soundPanel;

    private string[] lines;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        videoPlayer.loopPointReached += OnVideoEnd;

        leftLogField.gameObject.SetActive(false);
        rightLogField.gameObject.SetActive(false);
        titleText.SetActive(false);
        titleImage.SetActive(false);
        buttonText.SetActive(false);
        buttonBack.SetActive(false);
        optionPanel.SetActive(false);
        soundPanel.SetActive(false);

        string leftfilePath = "Assets/Text/Tittle_Log_LeftAccess.txt";
        leftFileText = AssetDatabase.LoadAssetAtPath<TextAsset>(leftfilePath).text;

        string rightfilePath = "Assets/Text/Tittle_Log_RightLog.txt";
        rightFileText = AssetDatabase.LoadAssetAtPath<TextAsset>(rightfilePath).text;

        lines = rightFileText.Split('\n');

        
    }

    /// <summary>
    /// 비디오 재생이 끝났을 때 비활성화하는 함수
    /// </summary>
    /// <param name="vp"></param>
    void OnVideoEnd(VideoPlayer vp)
    {
        rawImage.SetActive(false);
        StartCoroutine(LeftLog());
    }
    
    /// <summary>
    /// 좌측상단 로그 재생
    /// </summary>
    /// <returns></returns>
    IEnumerator LeftLog()
    {
        leftLogField.gameObject.SetActive(true);
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
        rightLogField.gameObject.SetActive(true);
        int currentIndex = 0; 
        rightLogField.text = ""; 

        while (currentIndex < lines.Length)
        {
            string line = lines[currentIndex];
            rightLogField.text += line;
            rightLogField.text += '\n';
            currentIndex++;

            scrollRect.verticalNormalizedPosition = 0.0f;

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
        yield return new WaitForSeconds(0.1f);

        titleText.SetActive(true);
        titleImage.SetActive(true);

        TextMeshProUGUI text = titleText.GetComponent<TextMeshProUGUI>();

        text.alpha = 0f;
        text.DOFade(1f, 2f).SetEase(Ease.InOutBounce);

        Image title = titleImage.GetComponent<Image>();

        title.color = new Color(1f, 1f, 1f, 0f);
        title.DOFade(1f, 2f).SetEase(Ease.InOutBounce);

        yield return new WaitForSeconds(0.1f);

        buttonText.SetActive(true);
        buttonBack.SetActive(true);

        App.instance.GetSoundManager().PlayBGM("BGM_TitleTheme");
    }
}
