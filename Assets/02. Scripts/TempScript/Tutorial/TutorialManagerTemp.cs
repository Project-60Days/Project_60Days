using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
//using DG.Tweening;

//public class TutorialManagerTemp : Singleton<TutorialManagerTemp>
//{
//    // 튜토리얼 씬 관리 스크립트

//    [Header("Tutorial")]
//    [SerializeField] public TutorialController tutorialController;

//    public Image lightBackground;
//    public bool isLightUp;

//    private void Start()
//    {
//        Init();
//    }

//    private void Init()
//    {
//        tutorialController.Init();
//        isLightUp = false;
//    }

//    public void LightUpBackground()
//    {
//        lightBackground.DOFade(0f, 2f).SetEase(Ease.InBounce).OnComplete(() =>
//        {
//            isLightUp = true;
//            lightBackground.gameObject.SetActive(false);
//        });
//    }

//    public void LightDownBackground()
//    {
//        lightBackground.gameObject.SetActive(true);
//        lightBackground.DOFade(0.8f, 2f).SetEase(Ease.InBounce).OnComplete(() =>
//        {
//            isLightUp = false;
//        });
//    }

//    public void EndTutorial()
//    {
//        // 튜토리얼 끝
//    }
//}
