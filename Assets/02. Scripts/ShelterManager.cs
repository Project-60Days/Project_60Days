using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ShelterManager : Manager
{
    [SerializeField] Image blackBlur;
    [SerializeField] CanvasGroup benchImg;
    [SerializeField] CanvasGroup benchDecoImg;
    [SerializeField] CanvasGroup mapImg;
    [SerializeField] Image batteryImg;
    [SerializeField] CubeCtrl cubeCtrl;
    [SerializeField] MainCamCtrl cameraCtrl;

    float lightUpDuration = 2f;

    int benchStartIndex;
    int mapStartIndex;

    protected override void Awake()
    {
        base.Awake();

        blackBlur.gameObject.SetActive(false);
        benchStartIndex = benchImg.transform.GetSiblingIndex();
        mapStartIndex = mapImg.transform.GetSiblingIndex();
    }

    #region Tutorial
    public void StartTutorial()
    {
        LightDownBackground();
        cubeCtrl.StopAnim();
        benchDecoImg.DOFade(0f, 0f);
    }

    public void LightUpBackground()
    {
        StartCoroutine(FillBattery(1f));

        blackBlur.DOFade(0f, lightUpDuration).OnComplete(() =>
        {
            benchDecoImg.DOFade(1f, lightUpDuration).SetEase(Ease.InOutBounce).OnComplete(() =>
            {
                cubeCtrl.StartAnim();
            });
            blackBlur.gameObject.SetActive(false);
        });
    }

    void LightDownBackground()
    {
        blackBlur.DOFade(0.95f, 0f).OnComplete(() =>
        {
            blackBlur.gameObject.SetActive(true);
            batteryImg.fillAmount = 0f;
        });
    }

    public void LightUpAndFillBattery(int _num)
    {
        float alpha = 0.25f * _num;

        StartCoroutine(FillBattery(alpha));

        blackBlur.DOFade(1 - alpha, lightUpDuration).SetEase(Ease.InBounce);
    }

    IEnumerator FillBattery(float _amount)
    {
        float timer = 0f;
        float initFill = batteryImg.fillAmount;
        float currentFill;
        float targetFill = _amount;

        while (timer < lightUpDuration)
        {
            timer += Time.deltaTime;
            currentFill = Mathf.Lerp(initFill, targetFill, timer / lightUpDuration);
            batteryImg.fillAmount = currentFill;

            yield return null;
        }

        batteryImg.fillAmount = targetFill;
    }

    public void LightUpWorkBench()
    {
        benchImg.DOFade(0.4f, 0f).OnComplete(() => benchImg.transform.SetAsLastSibling());
    }

    public void LightDownWorkBench()
    {
        benchImg.transform.SetSiblingIndex(benchStartIndex);

        benchImg.DOFade(1f, 0f);
    }

    public void LightUpMap()
    {
        mapImg.DOFade(0.6f, 0f).OnComplete(() => mapImg.transform.SetAsLastSibling());
    }

    public void LightDownMap()
    {
        mapImg.transform.SetSiblingIndex(mapStartIndex);

        mapImg.DOFade(1f, 0f);
    }
    #endregion

    #region Attack
    public void Attack()
    {
        cameraCtrl.Shake();
    }
    #endregion
}
