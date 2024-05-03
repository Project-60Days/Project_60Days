using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class PopUpPanel : UIBase
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
    string left = "���� ���� �����ϰ� 1���� ������.<br>ȭ���� �Һ����� ������ ���ô� ����� �Ǿ��� �����ڸ� ���Ƴ��� ���� �����ߴ� �η��� �� ���� �����ڸ� ������ �ĸ��� �¾Ҵ�.<br>��Ʈ��ũ�� Ÿ�� �ΰ��� Ĩ���� ���� ���̷����� 24�ð� ���� ���� �ֵ����� ���Ѱ�, ����ִ� ���� �ı��ϴ� �͸��� ������ ���� ���� �������.<br>�׵��� ���� ���� ������ �ڵ��� �����ڿ��� ������ ���� �װų�, �λ��� ���� �� �׵�� ���� ���� ��谡 �Ǿ���.<br> �׵��� �����ڿ� ���� �˾Ƴ� ���� �����ϴ� �͸����� �츮�� ���� ���� �ִ� Ĩ���� ���̷����� �±� ���۽�Ų�ٴ� �Ͱ�, �ִ��� �׵��� ���� ���� ���ƾ� �ߴ�.<br> �츮�� ������ ������ ���� �Ҹ� ���� ���׿� ������ �Դ�.<br> ������ �ֺ��� �����ڴ� ���� �̵��� ���ߴ�. <br>��ó �ֺ��� ���ڵ��� �����ϸ� ��������ó�� ������ �Դ�. <br>������ �ð��� �������� ������ ���� �ķ��� ���ڵ� ���� �� ������ ���� ���� ���� ������ ���� �Ǿ���. <br>����, �츮�� ������ ���� �������� ì�� �� 1�Ⱓ ���� �� ��ó�� ������. <br>���� �츮�� ����� ��� �ɱ�?";
    string right = "<i>������ ���� ���� ��...���������� �������� �簡�� �Ǿ����ϴ�.<br>���� ���� �ð����κ��� 368�� 4�ð� 38�� �������ϴ�.\n<i>A-48 �ý��� ���� �����մϴ�.<br>����� ����� ���������� �߰ߵǾ����ϴ�.<br>��󹰿� ���� �ý����� �籸���մϴ�.<br>������ �ܺ�ȯ����� �ٸ� Ư�̻����� �߰ߵǾ����ϴ�.\n<i>�� �ܺ�ȯ���� �м��Ͽ� ���� ���� �����մϴ�.<br>�м���� ���� 200M�� �Ĺ��� �߰�, ������ ��� ��ġ �̻��Դϴ�.\n<i>������� ���� �ý��� �� ���μ����� �籸���մϴ�.<br>���������� �ý����� �缳�� �Ǿ����ϴ�.<br>�ý��� A-48�� �޽þ� ���������� �簡���մϴ�.<br><i>���� ��ǥ ���� : ��Ʈ��ũ ��� ���� ���赵�� �����մϴ�.\n�ȳ��ϼ���.<br>���� <b><color=red>A-49</color></b>, �� �������� �ΰ����� �ý����Դϴ�.<br>������� �������� ������ ���� ����� �ý������� �����ǰ� ����ڸ� �ֱ������� �����մϴ�.<br>����ڿ��� ���� ��Ȳ�� �����մϴ�.<br>�ܺλ�Ȳ, �ʿ������ �����Ǹ� ���� �ֺ��� �����ҵ��� �߰ߵ��� �ʾҽ��ϴ�.<br>���λ�Ȳ, ��� �ջ����� ���� �������¿� ���� �ļ����� ���� �����Ҹ� �߰��߽��ϴ�.\n������� ���� ���� : ���忡�� ������ ���� ��� �� 18�� 42�� �������ϴ�.<br>���� ����, �ƹ� ����, ���� ����, ���� ���� �� �м���� �̻� �����ϴ�.<br>�ظ� �Ű� �κ� �̻� �߻�, �Ϻ� ����� �ջ�Ǿ����ϴ�.<br>������ ���̵� ���������� �����մϴ�.\n������� ����� �ջ����� ���� �������� ���� ���ɼ� 9.87% �̸�<br>����ڴ� �������� ������ �ʿ��� ���μ��� �н��� �����ؾ� �մϴ�.<br>�ȳ��� ���� �������� ����� �н��Ͻʽÿ�.\n�̰��� ����� �������Դϴ�.<br>����� DEAD.NET ������, �۸�ó���� ���ݹް� ���� ���� ����� ����Ͽ� ���鿡 ������ϴ�.\n����� ��� ���� �о��� �ϱ�� ����� ���� �����ϰ� �ִ� ���� ����Ʈ ��Ʈ�Դϴ�. �� ��Ʈ���� �����ҿ� ����� ������ ����� Ȯ���� �� �ֽ��ϴ�.";
    #endregion

    #region Override
    public override void Init()
    {
        backgroundImage = background.GetComponent<Image>();

        scrollbar.value = 1;

        backgroundImage.DOFade(0f, 0f);
        background.SetActive(false);
    }

    public override void ReInit() { }

    public override UIState GetUIState() => UIState.PopUp;

    public override bool IsAddUIStack() => true;

    public override void OpenPanel()
    {
        StartCoroutine(EndGamePopUp());
    }
    #endregion

    public IEnumerator EndGamePopUp()
    {
        App.Manager.UI.GetPanel<HighLightPanel>().ShowHighLight("Alert", "UI_NOTE");
        App.Manager.UI.GetPanel<AlertPanel>().SetAlert("caution", false);
        yield return new WaitUntil(() => App.Manager.UI.CurrState == UIState.Note);
        yield return new WaitUntil(() => App.Manager.UI.CurrState == UIState.Normal);
        base.OpenPanel();

        bgmVolume = App.Manager.Sound.SetBGMVolumeTweening(8f);
        App.Manager.Sound.StopSFX();

        text02.text = "���� ���丮 é�� 01 Ŭ������� " + App.Manager.Game.dayCount + "�� �ҿ�Ǿ����ϴ�.\n�����մϴ�!";

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
                App.Manager.Sound.SetBGMVolume(bgmVolume);
                App.Manager.Sound.PlayBGM("BGM_TitleTheme_Upgrade");
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
        ClosePanel();
        Application.Quit();
    }
}
