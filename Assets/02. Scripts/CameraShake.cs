using UnityEngine;
using TMPro;
using Cinemachine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera mainCamera;

    Color cyan = new Color(56f / 255f, 221f / 255f, 205f / 255f);

    public void Shake(TextMeshProUGUI _numberText)
    {
        PlaySFX();

        DecreaseDurabillityAnimation(_numberText);

        mainCamera.transform.DOShakePosition(1f);
    }

    void PlaySFX()
    {
        int sfxIndex = Random.Range(1, 5);
        App.instance.GetSoundManager().StopSFX();
        App.instance.GetSoundManager().PlaySFX("SFX_HIT_" + sfxIndex.ToString());
    }

    void DecreaseDurabillityAnimation(TextMeshProUGUI _numberText)
    {
        int endNumber = App.instance.GetMapManager().Controller.Player.Durability;

        _numberText.color = Color.red;

        int currentNumber = int.Parse(_numberText.text);
        DOTween.To(() => currentNumber, x => currentNumber = x, endNumber, 1f)
            .OnUpdate(() => _numberText.text = currentNumber.ToString())
            .OnComplete(() => _numberText.color = cyan);
    }
}