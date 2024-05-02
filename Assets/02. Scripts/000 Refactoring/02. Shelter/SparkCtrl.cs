using UnityEngine;

public class SparkCtrl : MonoBehaviour
{
    [Header("Interval")]
    [SerializeField] float minInterval = 15f;
    [SerializeField] float maxInterval = 45f;

    private Animator animator;

    private float timer = 0;
    private float interval;

    private void Start()
    {
        animator = GetComponent<Animator>();
 
        GenerateNextInterval();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer = 0f;
            PlayAnim();
            GenerateNextInterval();
        }
    }

    private void PlayAnim()
    {
        animator.SetTrigger("LightUp");
        PlaySFX();
    }

    private void PlaySFX()
    {
        if (App.Manager.UI.isUIStatus(UIState.Normal))
        {
            int sfxIndex = Random.Range(1, 5);
            if (App.Manager.Sound.CheckSFXPlayNow() == false)
                App.Manager.Sound.PlaySFX("SFX_SPARK_" + sfxIndex.ToString());
        }
    }

    /// <summary>
    /// Randomize the time to play the next animation
    /// </summary>
    private void GenerateNextInterval()
    {
        interval = Random.Range(minInterval, maxInterval);
    }
}
