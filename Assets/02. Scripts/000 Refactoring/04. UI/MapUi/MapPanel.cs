using UnityEngine;
using UnityEngine.UI;
using TMPro;

public struct TileInfo
{
    public Sprite img;
    public string landformTxt;
    public string resourceTxt;
    public string enemyTxt;
}

public class MapPanel : UIBase
{
    [SerializeField] GameObject disturbtorButton;
    [SerializeField] GameObject explorerButton;

    [SerializeField] GameObject tileInfo;
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI landformTMP;
    [SerializeField] TextMeshProUGUI resourceTMP;
    [SerializeField] TextMeshProUGUI enemyTMP;

    #region Override
    public override void Init()
    {
        gameObject.SetActive(false);
    }

    public override void ReInit() 
    {
        SetInfoActive(false);
    }
    #endregion

    public void DisturbtorButtonInteractableOn()
    {
        disturbtorButton.GetComponentInChildren<Button>().interactable = true;
    }

    public void SetInfo(TileInfo _info)
    {
        image.sprite = _info.img;
        landformTMP.text = _info.landformTxt;
        resourceTMP.text = _info.resourceTxt;
        enemyTMP.text = _info.enemyTxt;
    }

    public void SetInfoActive(bool _isActive)
    {
        tileInfo.SetActive(_isActive);
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
