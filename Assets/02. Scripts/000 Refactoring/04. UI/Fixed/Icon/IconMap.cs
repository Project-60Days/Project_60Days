using UnityEngine;
using UnityEngine.UI;

public class IconMap : IconBase
{
    [SerializeField] Sprite city;    
    [SerializeField] Sprite desert;
    [SerializeField] Sprite jungle;
    [SerializeField] Sprite tundra;

    private Image image;
    private Player player;

    private ETileType type;

    protected override void Start()
    {
        base.Start();

        image = GetComponent<Image>();
        player = App.Manager.Map.mapCtrl.Player;

        ResetIcon();
    }

    public void ResetIcon()
    {
        var tile = player.TileController;
        type = tile.GetComponent<TileBase>().TileType;

        image.sprite = SetImage();
        text = SetString();
    }

    protected override string SetString() => type switch
    {
        ETileType.City => App.Data.Game.GetString("STR_TILE_NONE_DESC"),
        ETileType.Desert => App.Data.Game.GetString("STR_TILE_DESERT_DESC"),
        ETileType.Jungle => App.Data.Game.GetString("STR_TILE_JUNGLE_DESC"),
        ETileType.Tundra => App.Data.Game.GetString("STR_TILE_TUNDRA_DESC"),
        _ => "",
    };

    private Sprite SetImage() => type switch
    {
        ETileType.City => city,
        ETileType.Desert => desert,
        ETileType.Jungle => jungle,
        ETileType.Tundra => tundra,
        _ => null,
    };
}
