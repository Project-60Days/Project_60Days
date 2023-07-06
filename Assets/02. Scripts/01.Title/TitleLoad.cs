using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Video;

public class TitleLoad : MonoBehaviour
{
    [SerializeField] GameObject rawImage;
    VideoPlayer videoPlayer;

    [SerializeField] Text leftLogField;
    string leftFileText;

    [SerializeField] ScrollRect scrollRect;
    [SerializeField] RectTransform contentRect;
    [SerializeField] Text rightLogField;
    string rightFileText;

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

    void OnVideoEnd(VideoPlayer vp)
    {
        rawImage.SetActive(false);
        StartCoroutine(LeftLog());
    }
    

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
                yield return new WaitForSeconds(0.1f);
            }
            
            currentIndex++;
        }

        StartCoroutine(RightLog());
    }

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

            yield return new WaitForSeconds(0.1f);
        }

        StartCoroutine(Title());
    }

    IEnumerator Title()
    {
        yield return new WaitForSeconds(0.1f);

        titleText.SetActive(true);
        titleImage.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        buttonText.SetActive(true);
        buttonBack.SetActive(true);
    }
}
