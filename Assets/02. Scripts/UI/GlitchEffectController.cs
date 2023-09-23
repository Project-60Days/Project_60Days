using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlitchEffectController : MonoBehaviour
{
    [SerializeField] private float minActiveTime;
    [SerializeField] private float maxActiveTime;
    [SerializeField] private float effectRemainTime;

    private Image image;
    private Material material;

    float nextTime;

    private void OnEnable()
    {
        image = GetComponent<Image>();
        material = image.material;

        nextTime = GetRandomTime(minActiveTime, maxActiveTime);

        StartCoroutine(EffectTimer());
    }

    IEnumerator EffectTimer()
    {
        yield return new WaitForSeconds(nextTime);

        nextTime = GetRandomTime(minActiveTime, maxActiveTime);

        OnEffect();

        yield return new WaitForSeconds(effectRemainTime);

        OffEffect();

        StartCoroutine(EffectTimer());
    }


    private float GetRandomTime(float min, float max)
    {
        float randTime = UnityEngine.Random.Range(min, max);

        return randTime;
    }

    private void OnEffect()
    {
        material.EnableKeyword("GLITCH_ON");
    }

    private void OffEffect()
    {
        material.DisableKeyword("GLITCH_ON");
    }
}
