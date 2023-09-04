using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NextDayBtn : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Gauage Obejcts")]
    [SerializeField] Image gaugeImage;
    [SerializeField] float fillSpeed = 1.0f;
    [SerializeField] float maxGaugeValue = 100.0f;
    float currentGaugeValue = 0.0f;
    bool isFilling = false;

    void Start()
    {
        InitGauageUI();
    }

    void Update()
    {
        if (isFilling) //버튼 게이지 관련
        {
            FillGauge();
            if (currentGaugeValue >= maxGaugeValue) //게이지가 다 차면 다음 날로 이동
            {
                isFilling = false;
                UIManager.instance.GetNextDayController().NextDayEvent();
            }
        }
    }

    /// <summary>
    /// 버튼 게이지 초기화
    /// </summary>
    void InitGauageUI()
    {
        currentGaugeValue = 0.0f;
        gaugeImage.fillAmount = 0;
    }

    /// <summary>
    /// 버튼을 누르고 있을 때
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        isFilling = true;
    }

    /// <summary>
    /// 버튼에서 뗐을 때
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        isFilling = false;
        InitGauageUI();
    }

    /// <summary>
    /// 게이지 채움
    /// </summary>
    void FillGauge()
    {
        currentGaugeValue += fillSpeed * Time.deltaTime;
        UpdateGaugeUI();
    }

    /// <summary>
    /// 게이지에 따라 Ui 변경
    /// </summary>
    void UpdateGaugeUI()
    {
        gaugeImage.fillAmount = currentGaugeValue / maxGaugeValue;
    }
}
