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

    /// <summary>
    /// Randomize the time to play the next animation
    /// </summary>
    private void GenerateNextInterval()
    {
        interval = Random.Range(minInterval, maxInterval);
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
        if (App.Manager.UI.CurrState == UIState.Normal)
        {
            int sfxIndex = Random.Range(1, 5);
            if (!App.Manager.Sound.IsPlayingSFX())
                App.Manager.Sound.PlaySFX("SFX_Spark_" + sfxIndex.ToString());
        }
    }
}
