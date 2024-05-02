using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapPanel : UIBase
{
    [SerializeField] GameObject disturbtorButton;
    [SerializeField] GameObject explorerButton;
    [SerializeField] TargetPointUI targetPoint;
    [SerializeField] TileInfoPanel tileInfoPanel;

    #region Override
    public override void Init()
    {
        gameObject.SetActive(false);
    }

    public override void ReInit() 
    {
        OffPlayerMovePoint();
    }
    #endregion

    public void DisturbtorButtonInteractableOn()
    {
        disturbtorButton.GetComponentInChildren<Button>().interactable = true;
    }

    public bool GetActiveTileInfo()
    {
        return tileInfoPanel.gameObject.activeInHierarchy;
    }

    public void TrueTileInfo()
    {
        // var screenPoint = Camera.main.WorldToScreenPoint(tilePos);
        // tileInfoPanel.transform.position = screenPoint;
        if (App.Manager.UI.isUIStatus(UIState.Map))
            tileInfoPanel.gameObject.SetActive(true);
    }

    public void FalseTileInfo()
    {
        tileInfoPanel.gameObject.SetActive(false);
    }

    public void MoveTileInfo(Vector3 pos)
    {
        tileInfoPanel.transform.localPosition = pos;
    }

    public bool GetTileInfoActivate()
    {
        return tileInfoPanel.gameObject.activeInHierarchy;
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

    public void UpdateImage(Sprite sprite)
    {
        tileInfoPanel.UpdateImage(sprite);
    }

    public bool MovePointActivate()
    {
        return targetPoint.ActivateStatus();
    }
    
    public void ExplorerButtonInteractable(bool isOn)
    {
        explorerButton.SetActive(isOn);
        explorerButton.GetComponentInChildren<Button>().interactable = isOn;
    }
    
    public void DistrubtorButtonInteractable(bool isOn)
    {
        disturbtorButton.SetActive(isOn);
        disturbtorButton.GetComponentInChildren<Button>().interactable = isOn;
    }
}
