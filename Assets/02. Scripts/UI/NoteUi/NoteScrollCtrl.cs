using UnityEngine;
using DG.Tweening;

public class NoteScrollCtrl : MonoBehaviour
{
    Vector3 startLocalPosition;
    float startPositionY;

    float moveDuration = 0.5f;

    
    void Start()
    {
        startLocalPosition = transform.localPosition;
        startPositionY = startLocalPosition.y;
        gameObject.SetActive(false);
    }

    public void StartAnim()
    {
        gameObject.SetActive(true);

        transform.localPosition = startLocalPosition;

        transform.DOLocalMoveY(startPositionY + 10f, moveDuration).SetLoops(-1, LoopType.Yoyo);
    }
    
    public void StopAnim()
    {
        transform.DOKill();

        gameObject.SetActive(false);
    }
}
