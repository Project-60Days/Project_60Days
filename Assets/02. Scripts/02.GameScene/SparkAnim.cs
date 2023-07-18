using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkAnim : MonoBehaviour
{
    [SerializeField] string animName;
    Animation sparkAnimation;

    float timer = 0;
    float interval = 5f;

    void Start()
    {
        sparkAnimation = GetComponent<Animation>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer = 0f;
            PlayAnimation();
        }
    }

    private void PlayAnimation()
    {
        sparkAnimation.Play(animName);
    }
}
