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
    [SerializeField] Button disruptorBtn;
    [SerializeField] Button explorerBtn;

    [Header("Tile Info Objects")]
    [SerializeField] GameObject tileInfo;
    [SerializeField] Image infoImg;
    [SerializeField] TextMeshProUGUI landformTMP;
    [SerializeField] TextMeshProUGUI resourceTMP;
    [SerializeField] TextMeshProUGUI enemyTMP;

    #region Override
    public override void Init()
    {
        ClosePanel();
    }

    public override void ReInit() 
    {
        SetInfoActive(false);
    }
    #endregion

    public void SetInfo(TileInfo _info)
    {
        infoImg.sprite = _info.img;
        landformTMP.text = _info.landformTxt;
        resourceTMP.text = _info.resourceTxt;
        enemyTMP.text = _info.enemyTxt;
    }

    public void SetInfoActive(bool _isActive)
    {
        tileInfo.SetActive(_isActive);
    }

    public void ActiveDroneBtn(DroneType _type, bool _isOn)
    {
        switch (_type)
        {
            case DroneType.Disruptor:
                disruptorBtn.gameObject.SetActive(_isOn);
                break;

            case DroneType.Explorer:
                explorerBtn.gameObject.SetActive(_isOn);
                break;
        }
    }
}
