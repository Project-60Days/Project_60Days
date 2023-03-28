using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClosedButton_Anim : MonoBehaviour
{
    public Button button;
    public Animator anim;
    public Animation OpenedAnim;
    public AnimationClip OpenedClip;
    public GameObject OpenedButton;

    private void Start()
    {
        OpenedButton = GameObject.Find("Opened_Button");
        OpenedButton.SetActive(false);

        this.anim.Play("Normal");

        button.onClick.AddListener(() =>
        {
            this.anim.speed = 1;
            this.anim.Play("Selected");
        });
    }

    /// <summary>
    /// ClosedButton_anim 끝난 후 Opened 애니메이션 클립 재생 이벤트 호출
    /// </summary>
    public void PlayOpenedAnimation()
    {
        OpenedButton.SetActive(true);
        OpenedAnim.clip = OpenedClip;
        OpenedAnim.Play();
    }
}
