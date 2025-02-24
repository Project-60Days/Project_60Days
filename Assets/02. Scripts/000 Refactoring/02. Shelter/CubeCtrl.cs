using UnityEngine;
using DG.Tweening;

public class CubeCtrl : MonoBehaviour
{
    [SerializeField] Transform cube;
    [SerializeField] Transform cubeElse;

    private float centerStartPositionY;
    private float elseStartPositionY;

    private void Start()
    {
        centerStartPositionY = cube.position.y;
        elseStartPositionY = cubeElse.position.y;

        StartAnim();
    }

    public void StartAnim()
    {
        cube.DOMoveY(centerStartPositionY + 1f, 5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        cubeElse.DOMoveY(elseStartPositionY + 2f, 5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }

    public void StopAnim()
    {
        cube.DOKill();
        cubeElse.DOKill();

        cube.DOMoveY(centerStartPositionY, 0f);
        cubeElse.DOMoveY(elseStartPositionY, 0f);
    }
}
