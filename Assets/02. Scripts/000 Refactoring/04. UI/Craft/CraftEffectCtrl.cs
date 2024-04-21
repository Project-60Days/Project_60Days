using UnityEngine;
using DG.Tweening;

public class CraftEffectCtrl : MonoBehaviour
{
    float moveDuration = 10.0f;

    float initPositionX;

    void Start()
    {
        initPositionX = transform.position.x;
    }

    public void StartAnim()
    {
        Vector3 newPosition = transform.position;
        newPosition.x = initPositionX;
        transform.position = newPosition;

        float screenWidth = Screen.width;

        transform.DOMoveX(screenWidth, moveDuration).SetLoops(-1, LoopType.Yoyo);
    }

    public void StopAnim()
    {
        transform.DOKill();
    }
}
