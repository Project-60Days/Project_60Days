using UnityEngine;
using TMPro;
using Cinemachine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    public CinemachineVirtualCamera mainCamera;
    Vector3 cameraPos;

    float shakeRange = 0.05f;
    float duration = 1f;

    // Start is called before the first frame update
    public void Shake(TextMeshProUGUI _numberText)
    {
        PlaySFX();
        DecreaseDurabillityAnimation(_numberText);

        cameraPos = mainCamera.transform.position;
        InvokeRepeating("StartShake", 0f, 0.005f);
        Invoke("StopShake", duration);
    }

    void PlaySFX()
    {
        int sfxIndex = Random.Range(1, 5);
        App.instance.GetSoundManager().StopSFX();
        App.instance.GetSoundManager().PlaySFX("SFX_HIT_" + sfxIndex.ToString());
    }

    void DecreaseDurabillityAnimation(TextMeshProUGUI _numberText)
    {
        int endNumber = App.instance.GetMapManager().mapController.Player.Durability;

        _numberText.color = Color.red;

        int currentNumber = int.Parse(_numberText.text);
        DOTween.To(() => currentNumber, x => currentNumber = x, endNumber, 1f)
            .OnUpdate(() => _numberText.text = currentNumber.ToString())
            .OnComplete(() => _numberText.color = Color.white);
    }

    void StartShake()
    {
        float cameraPosX = Random.value * shakeRange * 2 - shakeRange;
        float cameraPosY = Random.value * shakeRange * 2 - shakeRange;
        Vector3 cameraPos = mainCamera.transform.position;
        cameraPos.x += cameraPosX;
        cameraPos.y += cameraPosY;
        mainCamera.transform.position = cameraPos;
    }

    void StopShake()
    {
        CancelInvoke("StartShake");
        mainCamera.transform.position = cameraPos;
    }
}