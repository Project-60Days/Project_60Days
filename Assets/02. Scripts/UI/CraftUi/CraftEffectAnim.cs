using UnityEngine;
using DG.Tweening;

public class CraftEffectAnim : MonoBehaviour
{
    public bool isActive = false;

    float moveDistance = 1920f;
    float moveDuration = 10.0f;

    float initPositionX;

    void Awake()
    {
        initPositionX = transform.position.x;
    }

    public void Init()
    {
        Vector3 newPosition = transform.position;
        newPosition.x = initPositionX;
        transform.position = newPosition;

        isActive = true;

        MoveRight();
    }

    void MoveRight()
    {
        if (isActive == false)
            return;
        transform.DOMoveX(transform.position.x + moveDistance, moveDuration)//.SetEase(Ease.Linear)
           .OnComplete(MoveLeft);
    }

    void MoveLeft()
    {
        if (isActive == false)
            return;
        transform.DOMoveX(transform.position.x - moveDistance, moveDuration)//.SetEase(Ease.Linear)
            .OnComplete(MoveRight);
    }
}
