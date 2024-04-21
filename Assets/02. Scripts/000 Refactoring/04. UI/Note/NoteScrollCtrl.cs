using UnityEngine;
using DG.Tweening;

public class NoteScrollCtrl : MonoBehaviour
{
    [SerializeField] Vector3 startPosition;
    float startPositionY;

    float moveDuration = 0.5f;

    
    void Start()
    {
        startPositionY = startPosition.y;
        gameObject.SetActive(false);
    }

    public void StartAnim()
    {
        gameObject.SetActive(true);

        transform.localPosition = startPosition;

        transform.DOLocalMoveY(startPositionY + 10f, moveDuration).SetLoops(-1, LoopType.Yoyo);
    }
    
    public void StopAnim()
    {
        transform.DOKill();

        gameObject.SetActive(false);
    }
}
