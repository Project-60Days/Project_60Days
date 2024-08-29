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

    //private IEnumerator CheckScrollEnabled()
    //{
    //    scrollCtrl.StopAnim();

    //    int index = notePages[pageNum].GetPageType() == PageType.Result ? 0 : 1;
    //    scrollRects[index].verticalNormalizedPosition = 1.0f;

    //    yield return null;

    //    if (scrollBars[index].gameObject.activeSelf)
    //    {
    //        scrollCtrl.StartAnim();
    //        StartCoroutine(WaitScrollToEnd(scrollBars[index]));
    //    }
    //}

    //private IEnumerator WaitScrollToEnd(Scrollbar scrollBar)
    //{
    //    yield return new WaitUntil(() => scrollBar.value <= 0.1f);
    //    scrollCtrl.StopAnim();
    //}


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
