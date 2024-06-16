using UnityEngine;
using UnityEngine.UI;

public class IconMap : IconBase
{
    [SerializeField] Sprite city;    
    [SerializeField] Sprite desert;
    [SerializeField] Sprite jungle;
    [SerializeField] Sprite tundra;

    private Image image;

    private TileType type;

    protected override void Start()
    {
        base.Start();

        image = GetComponent<Image>();
    }

    public void ResetIcon(TileBase _tile)
    {
        type = _tile.GetTileType();

        image.sprite = GetImage();
        text = GetString();
    }

    protected override string GetString() => type switch
    {
        TileType.City => App.Data.Game.GetString("STR_TILE_CITY_DESC"),
        TileType.Desert => App.Data.Game.GetString("STR_TILE_DESERT_DESC"),
        TileType.Jungle => App.Data.Game.GetString("STR_TILE_JUNGLE_DESC"),
        TileType.Tundra => App.Data.Game.GetString("STR_TILE_TUNDRA_DESC"),
        _ => "",
    };

    private Sprite GetImage() => type switch
    {
        TileType.City => city,
        TileType.Desert => desert,
        TileType.Jungle => jungle,
        TileType.Tundra => tundra,
        _ => null,
    };
}
