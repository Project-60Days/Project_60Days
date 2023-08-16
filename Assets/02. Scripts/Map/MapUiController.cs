using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapUiController : ControllerBase
{
    [SerializeField] GameObject disturbtorButton;
    [SerializeField] GameObject explorerButton;
    [SerializeField] ArrowPointUI arrowPoint;
    [SerializeField] TileInfoPanel tileInfoPanel;

    public override EControllerType GetControllerType()
    {
        return EControllerType.MAP;
    }

    public void DisturbtorButtonInteractableOn()
    {
        disturbtorButton.GetComponentInChildren<Button>().interactable = true;
    }

    public void TileInfoPanelActive(bool _active)
    {
        tileInfoPanel.gameObject.SetActive(_active);
    }

    public void PlayerMovePointActive(Transform transform)
    {
        arrowPoint.OnEffect(transform);
    }

    public void UpdateText(ETileInfoTMP infoTMP, string text)
    {
        tileInfoPanel.UpdateText(infoTMP, text);
    }
}
