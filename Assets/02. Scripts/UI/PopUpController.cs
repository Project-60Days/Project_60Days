using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class PopUpController : MonoBehaviour
{
    [SerializeField] Scrollbar scrollbar;
    [SerializeField] Image logoImage;
    [SerializeField] GameObject background;
    [SerializeField] TextMeshProUGUI text01;
    [SerializeField] TextMeshProUGUI text02;
    [SerializeField] TextMeshProUGUI text03;
    [SerializeField] TextMeshProUGUI loginText;
    [SerializeField] TextMeshProUGUI leftText;
    [SerializeField] TextMeshProUGUI rightText;
    
    Image backgroundImage;

    bool isCompleteToPlayBGM = false;
    bool isComplete13f = false;
    bool isComplete19f = false;
    bool isCompleteToFadeIn = false;

    float bgmVolume;

    #region text
    string login = "Log in...\nAccess Key : ver_Omega_6a%d5k3j2i9\nAccess.  .  .Success\n=============================================\nID : OMEGA\nBOOT MODE : NORMAL\nCAUTION : ENTERING DEAD.NET..DATA LOADED 99%...\n=============================================\n - target: {fileID: 429962314220, guid: d1fc68b1b8c32bd41841fcd342, type: 3} propertyPath: m_AnchorMin.y value: 0 objectReference: {fileID: 0}\n- target: {fileID: 429962314220, guid: d1fc68b1b8c32bd41841fcd342, type: 3} propertyPath: m_AnchoredPosition.x value: 0 objectReference: {fileID: 0}\n - target: {fileID: 429962314220, guid: d1fc68b1b8c32bd41841fcd342, type: 3} propertyPath: m_AnchoredPosition.y value: 0 objectReference: {fileID: 0}\n - target: {fileID: 430069157147, guid: d1fc68b1b8c32bd41841fcd342, type: 3} propertyPath: m_AnchorMax.y\n<color=red> - Caution : YOU ENTERING TO DEAD.NET</color>";
    string left = "데드 넷이 등장하고 1년이 지났다.<br>화려한 불빛으로 빛나던 도시는 잿빛이 되었고 감염자를 몰아내기 위해 연합했던 인류는 더 많은 감염자를 만들어내며 파멸을 맞았다.<br>네트워크를 타고 인간의 칩으로 들어온 바이러스는 24시간 내로 몸의 주도권을 빼앗고, 살아있는 것을 파괴하는 것만이 본능인 살인 기계로 만들었다.<br>그들을 막기 위해 저항한 자들은 감염자에게 무참히 찢겨 죽거나, 부상을 입은 후 그들과 같은 살인 기계가 되었다.<br> 그동안 감염자에 대해 알아낸 것은 접촉하는 것만으로 우리의 뇌에 박혀 있는 칩으로 바이러스를 태그 전송시킨다는 것과, 최대한 그들의 눈에 띄지 말아야 했다.<br> 우리는 감염자 무리를 피해 소리 없이 숨죽여 생존해 왔다.<br> 다행히 주변의 감염자는 많이 이동한 듯했다. <br>거처 주변의 물자들을 수집하며 바퀴벌레처럼 생존해 왔다. <br>하지만 시간이 지날수록 비축해 놓은 식량과 물자도 거의 다 떨어져 이제 숨어 지낼 수만은 없게 되었다. <br>오늘, 우리는 최후의 생존 도구만을 챙긴 후 1년간 지내 온 거처를 떠났다. <br>이제 우리의 운명은 어떻게 될까?";
    string right = "<i>연구실 전력 가동 중...정상적으로 연구실이 재가동 되었습니다.<br>이전 가동 시간으로부터 368일 4시간 38분 지났습니다.\n<i>A-48 시스템 진단 시작합니다.<br>대상물의 변경된 설정사항이 발견되었습니다.<br>대상물에 따라 시스템을 재구축합니다.<br>설정된 외부환경과는 다른 특이사항이 발견되었습니다.\n<i>현 외부환경을 분석하여 설정 값을 조정합니다.<br>분석결과 주위 200M의 식물을 발견, 습도가 평균 수치 이상입니다.\n<i>결과값을 토대로 시스템 내 프로세스를 재구축합니다.<br>정상적으로 시스템이 재설정 되었습니다.<br>시스템 A-48의 메시아 프로토콜을 재가동합니다.<br><i>절대 목표 설정 : 네트워크 백신 개발 설계도를 복원합니다.\n안녕하세요.<br>저는 <b><color=red>A-49</color></b>, 본 연구실의 인공지능 시스템입니다.<br>대상자의 지속적인 생존을 위해 구축된 시스템으로 연구실과 대상자를 주기적으로 점검합니다.<br>대상자에게 현재 상황을 보고합니다.<br>외부상황, 초원지대로 예측되며 현재 주변에 위험요소들은 발견되지 않았습니다.<br>내부상황, 기기 손상으로 인한 누전상태와 유리 파손으로 인한 위험요소를 발견했습니다.\n대상자의 상태 검진 : 포드에서 장기수면 이후 깨어난 지 18분 42초 지났습니다.<br>혈압 정상, 맥박 정상, 수분 정상, 혈액 검출 후 분석결과 이상 없습니다.<br>해마 신경 부분 이상 발생, 일부 장기기억 손상되었습니다.<br>연구실 가이드 프로토콜을 진행합니다.\n대상자의 장기기억 손상으로 인한 지속적인 생존 가능성 9.87% 미만<br>대상자는 지속적인 생존에 필요한 프로세스 학습을 진행해야 합니다.<br>안내에 따라 연구실의 사용을 학습하십시오.\n이곳은 당신의 연구실입니다.<br>당신은 DEAD.NET 접속자, 글리처에게 공격받고 시험 중인 백신을 사용하여 영면에 들었습니다.\n당신이 깨어난 직후 읽었던 일기는 당신이 개인 소지하고 있는 원격 리모트 노트입니다. 이 노트에서 연구소에 명령을 내리고 기록을 확인할 수 있습니다.";
    #endregion

    void Start()
    {
        backgroundImage = background.GetComponent<Image>();

        scrollbar.value = 1;

        backgroundImage.DOFade(0f, 0f);
        background.SetActive(false);
    }

    public IEnumerator EndGamePopUp()
    {
        UIManager.instance.GetUIHighLightController().ShowHighLight("Alert", "UI_NOTE");
        UIManager.instance.GetAlertController().SetAlert("caution", false);
        yield return new WaitUntil(() => UIManager.instance.isUIStatus("UI_NOTE"));
        yield return new WaitUntil(() => UIManager.instance.isUIStatus("UI_NORMAL"));
        UIManager.instance.AddCurrUIName("UI_POPUP");

        bgmVolume = App.instance.GetSoundManager().SetBGMVolumeTweening(8f);
        App.instance.GetSoundManager().StopSFX();

        text02.text = "메인 스토리 챕터 01 클리어까지 " + UIManager.instance.GetNoteController().dayCount + "일 소요되었습니다.\n축하합니다!";

        PlayCredit();
    }

    void PlayCredit()
    {
        background.SetActive(true);

        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(0.5f)
                .Append(backgroundImage.DOFade(1f, 1f))
                .Append(text01.DOFade(1f, 1f))
                .Append(text02.DOFade(1f, 1f))
                .Append(text03.DOFade(1f, 1f))
                .AppendInterval(0.5f)
                .Append(DOTween.To(() => scrollbar.value, x => scrollbar.value = x, 0f, 15f).SetEase(Ease.Linear));


        sequence.OnUpdate(() =>
        {
            if (sequence.Elapsed() >= 13f && isCompleteToPlayBGM == false)
            {
                isCompleteToPlayBGM = true;
                App.instance.GetSoundManager().SetBGMVolume(bgmVolume);
                App.instance.GetSoundManager().PlayBGM("BGM_TitleTheme_Upgrade");
            }

            if (sequence.Elapsed() >= 13f && isComplete13f == false)
            {
                isComplete13f = true;
                loginText.DOText(login, 3f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    rightText.DOText(right, 8f).SetEase(Ease.Linear);
                });
            }

            if (sequence.Elapsed() >= 17f && isCompleteToFadeIn == false)
            {
                isCompleteToFadeIn = true;
                logoImage.DOFade(1f, 1f);
            }

            if (sequence.Elapsed() >= 19f && isComplete19f == false)
            {
                isComplete19f = true;
                leftText.DOText(left, 5f, true, ScrambleMode.Uppercase).SetEase(Ease.Linear);
            }
        });
    }

    public void ClickBackToMenu()
    {
        UIManager.instance.PopCurrUI();
        GameManager.instance.QuitGame();
    }
}
