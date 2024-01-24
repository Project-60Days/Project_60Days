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
    /// 랜덤한 시간마다 애니메이션 재생
    /// </summary>
    private void PlayAnimation()
    {
        FadeInSpark();
        animator.SetTrigger("LightUp");
    }

    /// <summary>
    /// 다음 애니메이션 재생할 시간 랜덤 지정
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

        if (UIManager.instance.isUIStatus("UI_NORMAL"))
        {
            int sfxIndex = Random.Range(1, 5);
            if (App.instance.GetSoundManager().CheckSFXPlayNow() == false) 
                App.instance.GetSoundManager().PlaySFX("SFX_SPARK_" + sfxIndex.ToString());
        }
    }
}
