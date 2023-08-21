using UnityEngine;

public class SparkAnim : MonoBehaviour
{
    [SerializeField] string animName;

    [Header("Interval")]
    [SerializeField] float minInterval = 5f;
    [SerializeField] float maxInterval = 15f;

    Animator animator;

    float timer = 0;
    float interval = 5f;

    void Start()
    {
        animator = GetComponent<Animator>();
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
        animator.SetTrigger(animName);
    }

    /// <summary>
    /// 다음 애니메이션 재생할 시간 랜덤 지정
    /// </summary>
    private void GenerateNextInterval()
    {
        interval = Random.Range(minInterval, maxInterval);
    }
}
