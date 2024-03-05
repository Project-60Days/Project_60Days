using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NoteScroll : MonoBehaviour
{
    Vector3 localPosition;

    bool isActive = false;

    float moveDuration = 0.5f;

    float initPositionY;
    
    void Awake()
    {
        localPosition = transform.localPosition;
        initPositionY = localPosition.y;
        gameObject.SetActive(false);
    }

    public void StartAnim()
    {
        gameObject.SetActive(true);

        transform.localPosition = localPosition;

        isActive = true;

        MoveUp();
    }

    void MoveUp()
    {
        if (isActive == false)
            return;
        transform.DOLocalMoveY(initPositionY + 10f, moveDuration).SetEase(Ease.OutQuad)
           .OnComplete(MoveDown);
    }

    void MoveDown()
    {
        if (isActive == false)
            return;
        transform.DOLocalMoveY(initPositionY, moveDuration).SetEase(Ease.InQuad)
            .OnComplete(MoveUp);
    }
    
    public void StopAnim()
    {
        isActive = false;
        gameObject.SetActive(false);
    }
}
