using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Yarn.Unity;
using static Yarn.Unity.Effects;
using DG.Tweening;

public class TutorialDialogue : MonoBehaviour
{

    public void Show()
    {
        Debug.Log("TutorialUI Show");
        GetComponent<Transform>().DOMove(new Vector2(0f, 0f), 0.3f).SetEase(Ease.InQuad);
    }

    public void Hide()
    {
        Debug.Log("TutorialUI Hide");
        GetComponent<Transform>().DOMove(new Vector2(0f, -400f), 0.3f).SetEase(Ease.OutQuad);
    }
}
