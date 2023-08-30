using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetNextDay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    //void NextDayEvent()
    //{
    //    UIManager.instance.GetNoteController().CloseNote();
    //    blackPanel.gameObject.SetActive(true);
    //    Sequence sequence = DOTween.Sequence();
    //    sequence.Append(blackPanel.DOFade(1f, 0.5f)).SetEase(Ease.InQuint)
    //        .AppendInterval(0.5f)
    //        .Append(blackPanel.DOFade(0f, 0.5f + 0.5f))
    //        .OnComplete(() => NextDayEventCallBack());
    //    sequence.Play();

    //}

    //void NextDayEventCallBack()
    //{
    //    blackPanel.gameObject.SetActive(false);
    //    dayText.text = "Day " + ++dayCount;
    //    isNewDay = true;
    //    App.instance.GetMapManager().AllowMouseEvent(true);
    //    MapController.instance.NextDay();
    //    pageNum = 0;
    //    GameManager.instance.SetPrioryty(false);
    //}
}
