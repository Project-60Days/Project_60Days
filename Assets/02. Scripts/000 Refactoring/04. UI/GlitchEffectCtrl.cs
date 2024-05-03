using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GlitchEffectCtrl : MonoBehaviour
{
    [SerializeField] float minActiveTime;
    [SerializeField] float maxActiveTime;
    [SerializeField] float effectRemainTime;

    private Image image;
    private Material material;

    private float nextTime;

    private void OnEnable()
    {
        image = GetComponent<Image>();
        material = image.material;

        nextTime = GetRandomTime();

        StartCoroutine(EffectTimer());
    }
    private float GetRandomTime()
    {
        return Random.Range(minActiveTime, maxActiveTime);
    }

    private IEnumerator EffectTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(nextTime);

            nextTime = GetRandomTime();

            OnEffect();

            yield return new WaitForSeconds(effectRemainTime);

            OffEffect();
        }
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
