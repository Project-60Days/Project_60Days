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
    [SerializeField] Button nextDayBtn;
    [SerializeField] Button shelterBtn;

    [Header("Tile Info Objects")]
    [SerializeField] GameObject tileInfo;
    [SerializeField] Image infoImg;
    [SerializeField] TextMeshProUGUI landformTMP;
    [SerializeField] TextMeshProUGUI resourceTMP;
    [SerializeField] TextMeshProUGUI enemyTMP;

    #region Override
    public override UIState GetUIState() => UIState.Map;

    public override bool IsAddUIStack() => true;

    public override void Init()
    {
        SetButtonEvent();
        ClosePanel();
    }

    public override void OpenPanel()
    {
        base.OpenPanel();

        SetInfoActive(false);
    }
    #endregion

    public void UpdateTileInfo(TileInfo _info)
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

    public void SetActive(bool _isActive)
    {
        if (_isActive)
        {
            OpenPanel();
        }
        else
        {
            ClosePanel();
        }
    }

    private void SetButtonEvent()
    {
        nextDayBtn.onClick.AddListener(() => App.Manager.UI.FadeIn(() => App.Manager.Event.PostEvent(EventCode.NextDayStart, this)));
        shelterBtn.onClick.AddListener(() => App.Manager.Event.PostEvent(EventCode.GoToShelter, this));
    }

    public void SetBtnEnabled(bool _isActive)
    {
        nextDayBtn.enabled = _isActive;
        shelterBtn.enabled = _isActive;
    }
}
