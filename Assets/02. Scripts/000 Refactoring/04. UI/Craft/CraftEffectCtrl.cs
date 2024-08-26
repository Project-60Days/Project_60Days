using UnityEngine;
using DG.Tweening;

public class CraftEffectCtrl : MonoBehaviour
{
    private float startPositionX;

    private void Start()
    {
        startPositionX = transform.position.x;

        StartAnim();
    }

    private void StartAnim()
    {
        float screenWidth = Screen.width;

        transform.DOMoveX(screenWidth, 10f).SetLoops(-1, LoopType.Yoyo);
    }

    private void StopAnim()
    {
        transform.DOKill();

        transform.DOMoveX(startPositionX, 0f);
    }
}
