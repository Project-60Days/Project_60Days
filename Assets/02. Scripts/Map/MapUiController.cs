using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapUiController : ControllerBase
{
    [SerializeField] GameObject disturbtorButton;
    [SerializeField] GameObject explorerButton;
    [SerializeField] TargetPointUI targetPoint;
    [SerializeField] TileInfoPanel tileInfoPanel;

    public override EControllerType GetControllerType()
    {
        return EControllerType.MAP;
    }

    public void DisturbtorButtonInteractableOn()
    {
        disturbtorButton.GetComponentInChildren<Button>().interactable = true;
    }

    public bool GetActiveTileInfo()
    {
        return tileInfoPanel.gameObject.activeInHierarchy;
    }

    public void SetActiveTileInfo(bool _active)
    {
        tileInfoPanel.gameObject.SetActive(_active);
    }

    public void MoveTileInfo(Vector3 pos)
    {
        tileInfoPanel.transform.localPosition = pos;
    }

    public void OnPlayerMovePoint(Transform transform)
    {
        targetPoint.OnEffect(transform);
    }

    public void OffPlayerMovePoint()
    {
        targetPoint.OffEffect();
    }

    public void UpdateText(ETileInfoTMP infoTMP, string text)
    {
        tileInfoPanel.UpdateText(infoTMP, text);
    }
}
