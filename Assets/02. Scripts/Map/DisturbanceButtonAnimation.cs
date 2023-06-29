using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DisturbanceButtonAnimation : MonoBehaviour
{
    [SerializeField] RectTransform[] buttons;
    bool isClosed = true;

    public void ButtonAnimation()
    {
        if (isClosed)
        {
            for (int i = buttons.Length; i > 0; i--)
            {
                buttons[i - 1].DOAnchorPosY(140f * i, 0.5f);
            }
            isClosed = false;
        }
        else
        {
            for (int i = buttons.Length; i > 0; i--)
            {
                buttons[i - 1].DOAnchorPosY(gameObject.GetComponent<RectTransform>().position.y - 45f, 0.5f);
            }
            isClosed = true;
        }
    }
}
