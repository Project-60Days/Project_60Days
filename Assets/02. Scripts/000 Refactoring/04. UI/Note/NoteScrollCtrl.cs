using UnityEngine;
using DG.Tweening;

public class NoteScrollCtrl : MonoBehaviour
{
    private float startPositionY;

    private void Start()
    {
        startPositionY = transform.position.y;

        gameObject.SetActive(false);
    }

    public void StartAnim()
    {
        gameObject.SetActive(true);

        transform.DOMoveY(startPositionY + 10f, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }
    
    public void StopAnim()
    {
        transform.DOKill();

        transform.DOMoveY(startPositionY, 0f)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
    }
}
