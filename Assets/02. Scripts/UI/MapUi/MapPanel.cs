using UnityEngine;
using UnityEngine.UI;

public class MapPanel : UIBase
{
    [SerializeField] GameObject disturbtorButton;
    [SerializeField] GameObject explorerButton;
    [SerializeField] TileInfoCtrl tileInfoCtrl;

    #region Override
    public override void Init()
    {
        gameObject.SetActive(false);
    }

    public override void ReInit() 
    {
        TileInfo(false);
    }
    #endregion

    public void DisturbtorButtonInteractableOn()
    {
        disturbtorButton.GetComponentInChildren<Button>().interactable = true;
    }

    public void TileInfo(bool _isActive)
    {
        tileInfoCtrl.gameObject.SetActive(_isActive);
    }

    public void UpdateText(TileInfo infoTMP, string text)
    {
        tileInfoCtrl.SetText(infoTMP, text);
    }

    public void UpdateImage(Sprite sprite)
    {
        tileInfoCtrl.SetImage(sprite);
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
