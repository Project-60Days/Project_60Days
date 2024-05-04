using UnityEngine;
using DG.Tweening;

public class FloatingEffectCtrl : MonoBehaviour
{
    private void Start()
    {
        transform.DOMoveY(transform.parent.position.y + 0.4f, 5).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }
}
