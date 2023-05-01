using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DepthScroll : MonoBehaviour
{
    public Transform[] panels;
    public Image[] images;

    private int currentPanel = 0;

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f) // 마우스 휠을 위로 스크롤한 경우
        {
            if (currentPanel > 0)
            {
                currentPanel--; // 이전 패널로 이동

                Sequence sequence = DOTween.Sequence();
                sequence.Append(panels[currentPanel + 1].DOScale(1.5f, 1f).SetEase(Ease.InQuad))
                    .Append(images[currentPanel + 1].DOFade(0f, 0.5f))
                    .Join(images[currentPanel].DOFade(1f, 0.5f))
                    .Join(panels[currentPanel].DOScale(1.25f, 0.01f));

                panels[currentPanel + 1].DOKill();
                panels[currentPanel].DOKill();

                sequence.Play();

            }
        }
        else if (scroll < 0f) // 마우스 휠을 아래로 스크롤한 경우
        {
            if (currentPanel < panels.Length - 1)
            {
                currentPanel++; // 다음 패널로 이동

                Sequence sequence = DOTween.Sequence();
                sequence.Append(panels[currentPanel - 1].DOScale(1f, 1f).SetEase(Ease.InQuad))
                    .Append(images[currentPanel - 1].DOFade(0f, 0.5f))
                    .Join(images[currentPanel].DOFade(1f, 0.5f))
                    .Join(panels[currentPanel].DOScale(1.25f, 0.01f));
                
                panels[currentPanel - 1].DOKill();
                panels[currentPanel].DOKill();

                sequence.Play();
            }
        }
    }
}