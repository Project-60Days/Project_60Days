using UnityEngine;
using DG.Tweening;

public class CraftEffectAnim : MonoBehaviour
{
    public bool isActive = false;

    float moveDuration = 10.0f;

    float initPositionX;

    void Awake()
    {
        initPositionX = transform.position.x;
    }

    public void Init()
    {
        transform.DOKill();

        Vector3 newPosition = transform.position;
        newPosition.x = initPositionX;
        transform.position = newPosition;

        isActive = true;

        MoveRight();
    }

    void MoveRight()
    {
        float screenWidth = Screen.width;

        if (isActive == false)
            return;
        transform.DOMoveX(screenWidth, moveDuration)//.SetEase(Ease.Linear)
           .OnComplete(MoveLeft);
    }

    void MoveLeft()
    {
        if (isActive == false)
            return;
        transform.DOMoveX(initPositionX, moveDuration)//.SetEase(Ease.Linear)
            .OnComplete(MoveRight);
    }
}
