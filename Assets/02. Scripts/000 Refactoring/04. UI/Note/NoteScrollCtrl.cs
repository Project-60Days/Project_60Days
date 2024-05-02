using UnityEngine;
using DG.Tweening;

public class NoteScrollCtrl : MonoBehaviour
{
    [SerializeField] Vector3 startPosition;

    float startPositionY;

    void Start()
    {
        startPositionY = startPosition.y;
        gameObject.SetActive(false);
    }

    public void StartAnim()
    {
        gameObject.SetActive(true);

        transform.DOLocalMoveY(startPositionY + 10f, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }
    
    public void StopAnim()
    {
        transform.DOKill();

        transform.localPosition = startPosition;

        gameObject.SetActive(false);
    }
}
