using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartSceneButtonController : MonoBehaviour
{
    private Animator animator;
    public string clipName;

    private bool isPlaying = false;

    void Start()
    {
        animator = GetComponentInParent<Animator>();
    }

    public void OnButtonClick()
    {
        if (!isPlaying)
        {
            animator.Play(clipName);
            isPlaying = true;
        }
    }

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    if (eventData.pointerPress == gameObject)
    //    {
    //        animator.Play(clipName);
    //        isPlaying = true;
    //    }
    //}

    void Update()
    {
        if (isPlaying && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            animator.Play(0);
            isPlaying = false;
        }
    }
    private void OnEnable()
    {
        AnimationEvent animationEvent = new AnimationEvent();
        animationEvent.functionName = "StopAnimation";
        animationEvent.time = animator.runtimeAnimatorController.animationClips[0].length;
        animator.runtimeAnimatorController.animationClips[0].AddEvent(animationEvent);
    }

    private void StopAnimation()
    {
        animator.StopPlayback();
    }
}