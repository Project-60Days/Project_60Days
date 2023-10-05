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

    private void Start()
    {
        App.instance.AddController(this);
    }

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

    /// <summary>
    /// 마우스 위치에 따라 타일 인포 패널 다른 위치에 출력
    /// </summary>
    public void TrueTileInfo()
    {
        // var screenPoint = Camera.main.WorldToScreenPoint(tilePos);
        // tileInfoPanel.transform.position = screenPoint;
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

    public bool MovePointActivate()
    {
        return targetPoint.ActivateStatus();
    }
}
