using UnityEngine;
using UnityEngine.UI;

public class IconMap : IconBase, IListener
{
    protected override string GetString() => description;

    [SerializeField] Sprite[] tileSprites;

    private Image image;
    private string description;

    private void Awake()
    {
        App.Manager.Event.AddListener(EventCode.TileUpdate, this);
    }

    public void OnEvent(EventCode _code, Component _sender, object _param = null)
    {
        switch (_code)
        {
            case EventCode.TileUpdate:
                ResetIcon(_param as TileBase);
                break;
        }
    }

    protected override void Start()
    {
        base.Start();

        image = GetComponent<Image>();
    }

    private void ResetIcon(TileBase _tile)
    {
        image.sprite = tileSprites[(int)_tile.GetTileType()];
        text = GetString();

        description = App.Data.Game.GetString(_tile.tileData.Description);
    }
}
