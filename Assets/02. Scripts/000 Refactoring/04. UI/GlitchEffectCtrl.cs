using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GlitchEffectCtrl : MonoBehaviour
{
    [SerializeField] float minInterval = 5f;
    [SerializeField] float maxInterval = 8f;
    [SerializeField] float effectRemainTime = 1f;

    private Material material;

    private float interval;

    private void OnEnable()
    {
        material = GetComponent<Image>().material;

        GenerateNextInterval();
        StartCoroutine(EffectTimer());
    }

    private void GenerateNextInterval()
    {
        interval = Random.Range(minInterval, maxInterval);
    }

    private IEnumerator EffectTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);

            GenerateNextInterval();

            material.EnableKeyword("GLITCH_ON"); // Effect On

            yield return new WaitForSeconds(effectRemainTime);

            material.DisableKeyword("GLITCH_ON"); // Effect Off
        }
    }
}
