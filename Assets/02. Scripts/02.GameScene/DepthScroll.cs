using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DepthScroll : MonoBehaviour
{
    [SerializeField] Transform[] panels;
    [SerializeField] Image[] images;

    private int currentPanel = 0;

    [Header("Animation Settings")]
    [SerializeField] private float panelScaleUpDuration = 1f;
    [SerializeField] private float imageFadeDuration = 0.5f;
    [SerializeField] private float panelScaleDownDuration = 0.01f;

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f) // 마우스 휠을 위로 스크롤한 경우
        {
            if (currentPanel > 0)
            {
                ChangePanel(currentPanel - 1);
            }
        }
        else if (scroll < 0f) // 마우스 휠을 아래로 스크롤한 경우
        {
            if (currentPanel < panels.Length - 1)
            {
                ChangePanel(currentPanel + 1);
            }
        }
    }

    private void ChangePanel(int newPanelIndex)
    {
        currentPanel = Mathf.Clamp(newPanelIndex, 0, panels.Length - 1);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(AnimatePanelScale(panels[currentPanel + 1], 1.5f, panelScaleUpDuration))
            .Append(AnimateImageFade(images[currentPanel + 1], 0f, imageFadeDuration))
            .Join(AnimateImageFade(images[currentPanel], 1f, imageFadeDuration))
            .Join(AnimatePanelScale(panels[currentPanel], 1.25f, panelScaleDownDuration));

        panels[currentPanel + 1].DOKill();
        panels[currentPanel].DOKill();

        sequence.Play();
    }

    /// <summary>
    /// Panel 크기 변경 애니메이션
    /// </summary>
    /// <param name="panel"></param>
    /// <param name="targetScale"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private Tweener AnimatePanelScale(Transform panel, float targetScale, float duration)
    {
        return panel.DOScale(targetScale, duration).SetEase(Ease.InQuad);
    }

    /// <summary>
    /// Image fade 변경 애니메이션
    /// </summary>
    /// <param name="image"></param>
    /// <param name="targetAlpha"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private Tweener AnimateImageFade(Image image, float targetAlpha, float duration)
    {
        return image.DOFade(targetAlpha, duration);
    }
}