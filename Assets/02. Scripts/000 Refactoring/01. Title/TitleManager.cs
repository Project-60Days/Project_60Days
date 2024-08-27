using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class TitleManager : MonoBehaviour
{
    [Header("Text Production")]
    [SerializeField] TextMeshProUGUI leftTxt;
    [SerializeField] TextMeshProUGUI rightTxt;
    [SerializeField] ScrollRect rightScrollRect;
    [SerializeField] float duration = 2f;

    [Header("UI")]
    [SerializeField] GameObject titleBack;
    [SerializeField] GameObject buttonBack;
    [SerializeField] Image titleImage;
    [SerializeField] GameObject loadingImg;

    [Header("Buttons")]
    [SerializeField] Button newGameBtn;
    [SerializeField] Button quitBtn;

    private const string leftfilePath = "Text/Tittle_Log_LeftAccess";
    private const string rightfilePath = "Text/Tittle_Log_RightLog";

    private string leftFileText;
    private string rightFileText;

    private string[] lines;

    private void Start()
    {
        SetButtonEvent();
        LoadTextFiles();

        InitObjects();

#if UNITY_EDITOR
        buttonBack.SetActive(true);
        return;
#endif
        PlayLeftLog();
    }

    private void SetButtonEvent()
    {
        newGameBtn.onClick.AddListener(() =>
        {
            loadingImg.SetActive(true);

            App.Manager.Sound.StopBGM();

            App.LoadScene(SceneName.Shelter);
            App.LoadSceneAdditive(SceneName.Craft);
            App.LoadSceneAdditive(SceneName.UI);
            App.LoadSceneAdditive(SceneName.Map);
        });
    }

    private void LoadTextFiles()
    {
        leftFileText = Resources.Load<TextAsset>(leftfilePath).text;
        rightFileText = Resources.Load<TextAsset>(rightfilePath).text;

        lines = rightFileText.Split('\n');
    }

    private void InitObjects()
    {
        titleBack.SetActive(false);
        buttonBack.SetActive(false);
    }

    #region Title Production
    /// <summary>
    /// Play log production in the upper "left" corner
    /// </summary>
    private void PlayLeftLog()
    {
        leftTxt.DOText(leftFileText, duration)
            .SetEase(Ease.InSine)
            .OnComplete(() => StartCoroutine(PlayRightLog()));
    }

    /// <summary>
    /// Play log production in the upper "right" corner
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayRightLog()
    {
        rightTxt.text = string.Empty;
        float rightLogShowInterval = duration / lines.Length;

        foreach (string line in lines)
        {
            rightTxt.text += line + '\n';
            rightScrollRect.verticalNormalizedPosition = 0f;
            yield return new WaitForSeconds(rightLogShowInterval);
        }

        PlayLogo();
    }

    /// <summary>
    /// Play "logo" production
    /// </summary>
    /// <returns></returns>
    private void PlayLogo()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.AppendInterval(1f)
            .AppendCallback(() =>
            {
                titleBack.SetActive(true);

                titleImage.DOFade(0f, 1f).SetEase(Ease.Linear).From();

                App.Manager.Sound.PlayBGM("BGM_Title");
            })
            .AppendInterval(2f)
            .OnComplete(() =>
            {
                buttonBack.SetActive(true);
            });
    }
    #endregion
}
