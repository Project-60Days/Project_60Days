using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PopUpController : MonoBehaviour
{
    [SerializeField] Scrollbar scrollbar;
    [SerializeField] Image logoImage;

    bool isCompleteToPlayBGM = false;
    bool isCompleteToFadeIn = false;

    float bgmVolume;

    void Start()
    {
        scrollbar.value = 1;
        gameObject.SetActive(false);
    }

    public void EndGamePopUp()
    {
        UIManager.instance.AddCurrUIName("UI_POPUP");

        bgmVolume = App.instance.GetSoundManager().SetBGMVolumeTweening(8f);
        App.instance.GetSoundManager().StopSFX();

        gameObject.SetActive(true);

        StartCoroutine(PlayCredit());
    }

    IEnumerator PlayCredit()
    {
        yield return new WaitForSeconds(1f);


        var tween = DOTween.To(() => scrollbar.value, x => scrollbar.value = x, 0f, 10f).SetEase(Ease.Linear);

        tween.OnUpdate(() =>
        {
            if (tween.Elapsed() >= 7f && isCompleteToPlayBGM == false)
            {
                isCompleteToPlayBGM = true;
                App.instance.GetSoundManager().SetBGMVolume(bgmVolume);
                App.instance.GetSoundManager().PlayBGM("BGM_TitleTheme");
            }

            if (tween.Elapsed() >= 8f && isCompleteToFadeIn == false)
            {
                isCompleteToFadeIn = true;
                logoImage.DOFade(1f, 1f);
            }
        });
    }

    public void ClickBackToMenu()
    {
        UIManager.instance.PopCurrUI();
        GameManager.instance.QuitGame();
    }
}
