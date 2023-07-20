using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SparkAnim : MonoBehaviour
{
    [SerializeField] string animName;
    Animator animator;
    Image image;

    public float minInterval = 5f;
    public float maxInterval = 15f;

    float timer = 0;
    float interval = 5f;

    void Start()
    {
        animator = GetComponent<Animator>();
        image = GetComponent<Image>();
        GenerateNextInterval();

        Color imageColor = image.color;
        imageColor.a = 0f;
        image.color = imageColor;
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

    private void PlayAnimation()
    {
        Color imageColor = image.color;
        imageColor.a = 1f;
        image.color = imageColor;
        animator.SetTrigger(animName);
    }

    private void GenerateNextInterval()
    {
        interval = Random.Range(minInterval, maxInterval);
    }
}
