using DG.Tweening;
using UnityEngine;
using static NotePage;

public class MapInteraction : MonoBehaviour
{
    [SerializeField] Transform implant;

    Vector2 implantOriginalPos;

    void Start()
    {
        implantOriginalPos = implant.transform.position;

        //ImplantOpenEvent += ImplantOpenAnim;
        //ImplantCloseEvent += ImplantCloseAnim;
        App.instance.GetSoundManager().PlayBGM("BGM_InGameTheme");
    }

    /// <summary>
    /// 임플란트 열리는 애니메이션
    /// </summary>
    public void ImplantOpenAnim()
    {
        implant.DOMoveX(implantOriginalPos.x + 220f, 0.5f);
    }

    /// <summary>
    /// 임플란트 닫히는 애니메이션
    /// </summary>
    public void ImplantCloseAnim()
    {
        implant.DOMoveX(implantOriginalPos.x, 0.5f);
    }
}
