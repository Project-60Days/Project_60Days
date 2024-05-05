using UnityEngine;

public class ArrowCtrl : MonoBehaviour
{
    [SerializeField] GameObject arrowPrefab;

    private GameObject arrow;

    public void Init()
    {
        arrow = Instantiate(arrowPrefab, Vector3.zero, Quaternion.identity);
        arrow.SetActive(false);
    }

    public void ReInit()
    {
        arrow.SetActive(false);
    }

    public void ArrowOn(Vector3 _pos)
    {
        App.Manager.Sound.PlaySFX("SFX_Map_Select");

        _pos.y += 1f;
        arrow.transform.position = _pos;

        arrow.SetActive(true);
    }

    public void ArrowOff()
    {
        if (arrow.activeInHierarchy)
        {
            App.Manager.Sound.PlaySFX("SFX_Map_Cancel");
            arrow.SetActive(false);
        }
    }

    public bool CheckOn()
    {
        return arrow.activeInHierarchy;
    }
}
