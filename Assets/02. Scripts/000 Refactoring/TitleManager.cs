using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using DG.Tweening;
using TMPro;

public class TitleManager : MonoBehaviour
{
    [Header("Text Production")]
    [SerializeField] TextMeshProUGUI leftLogField;
    [SerializeField] TextMeshProUGUI rightLogField;
    [SerializeField] ScrollRect rightLogScrollRect;
    [SerializeField] float rightLogShowInterval;

    [Header("Title Objects")]
    [SerializeField] GameObject titleText;
    [SerializeField] Image titleImage;

    [SerializeField] GameObject buttonText;
    [SerializeField] GameObject buttonBack;

    string leftfilePath = "Text/Tittle_Log_LeftAccess";
    string rightfilePath = "Text/Tittle_Log_RightLog";

    string leftFileText;
    string rightFileText;

    string[] lines;

    void Start()
    {
        InitText();
        InitObjects();

        LeftLog();
    }

    #region Init
    void InitText()
    {
        leftFileText = Resources.Load<TextAsset>(leftfilePath).text;
        rightFileText = Resources.Load<TextAsset>(rightfilePath).text;

        lines = rightFileText.Split('\n');
    }

    void InitObjects()
    {
        ActiveTitleObjects(false);
        ActiveButtonObjects(false);
    }
    #endregion

    void ActiveTitleObjects(bool _isActive)
    {
        titleText.SetActive(_isActive);
        titleImage.gameObject.SetActive(_isActive);
    }

    void ActiveButtonObjects(bool _isActive)
    {
        buttonText.SetActive(_isActive);
        buttonBack.SetActive(_isActive);
    }

    #region Title Production
    /// <summary>
    /// Play log production in the upper "left" corner
    /// </summary>
    void LeftLog()
    {
        leftLogField.DOText(leftFileText, 2f)
            .SetEase(Ease.InSine)
            .OnComplete(() =>
            {
                StartCoroutine(RightLog());
            });
    }

    /// <summary>
    /// Play log production in the upper "right" corner
    /// </summary>
    /// <returns></returns>
    IEnumerator RightLog()
    {
        int currentIndex = 0;
        rightLogField.text = "";

        while (currentIndex < lines.Length)
        {
            string line = lines[currentIndex++];
            rightLogField.text += line + '\n';

            rightLogScrollRect.verticalNormalizedPosition = 0.0f;

            yield return new WaitForSeconds(rightLogShowInterval);
        }

        Logo();
    }

    /// <summary>
    /// Play logo production
    /// </summary>
    /// <returns></returns>
    void Logo()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(1f)
            .AppendCallback(() =>
            {
                ActiveTitleObjects(true);

                titleImage.DOFade(1f, 0f).SetEase(Ease.Linear).From();

                App.Manager.Sound.PlayBGM("BGM_TitleTheme");
            })
            .AppendInterval(2f)
            .OnComplete(() =>
            {
                ActiveButtonObjects(true);
            });
    }
    #endregion
}
