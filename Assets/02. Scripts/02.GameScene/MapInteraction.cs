using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;
using static NotePage;

public class MapInteraction : MonoBehaviour
{
    [SerializeField] Transform implant;
    [SerializeField] Sprite[] batteryImages;

    Vector2 implantOriginalPos;

    void Start()
    {
        implantOriginalPos = implant.transform.position;

        ImplantOpenEvent += ImplantOpenAnim;
        ImplantCloseEvent += ImplantCloseAnim;
    }


    public void ImplantOpenAnim()
    {
        implant.DOMoveX(implantOriginalPos.x + 220f, 0.5f);
    }
    public void ImplantCloseAnim()
    {
        implant.DOMoveX(implantOriginalPos.x, 0.5f);
    }
}
