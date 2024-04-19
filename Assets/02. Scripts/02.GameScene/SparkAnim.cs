using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SparkAnim : MonoBehaviour
{
    [Header("Interval")]
    [SerializeField] float minInterval = 15f;
    [SerializeField] float maxInterval = 45f;

    Animator animator;

    Image image;

    float timer = 0;
    float interval;

    bool isFirst = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        image = GetComponent<Image>();
        FadeOutSpark();
        GenerateNextInterval();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer = 0f;
            PlayAnimation();
            GenerateNextInterval();
        }
    }

    /// <summary>
    /// ������ �ð����� �ִϸ��̼� ���
    /// </summary>
    private void PlayAnimation()
    {
        FadeInSpark();
        animator.SetTrigger("LightUp");
    }

    /// <summary>
    /// ���� �ִϸ��̼� ����� �ð� ���� ����
    /// </summary>
    private void GenerateNextInterval()
    {
        interval = Random.Range(minInterval, maxInterval);
    }

    public void FadeInSpark()
    {
        image.DOFade(1f, 0f);
    }

    public void FadeOutSpark()
    {
        image.DOFade(0f, 0f);
    }

    public void PlaySparkSFX()
    {
        if (isFirst == true)
        {
            isFirst = false;
            return;
        }

        if (App.Manager.UI.isUIStatus(UIState.Normal))
        {
            int sfxIndex = Random.Range(1, 5);
            if (App.Manager.Sound.CheckSFXPlayNow() == false)
                App.Manager.Sound.PlaySFX("SFX_SPARK_" + sfxIndex.ToString());
        }
    }
}
