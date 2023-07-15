using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InventoryAnimation : MonoBehaviour
{
    Sequence sequence;
    Vector3 pos;

    private void Start()
    {
        pos = transform.position;
    }

    public void CraftingAnimation()
    {
        sequence = DOTween.Sequence();

        sequence
            .OnStart(() => {
                gameObject.SetActive(true);
                transform.position = pos;
            })
            .AppendInterval(0.5f)
            .Append(transform.DOMoveX(pos.x + 950f, 1f));
    }
}
