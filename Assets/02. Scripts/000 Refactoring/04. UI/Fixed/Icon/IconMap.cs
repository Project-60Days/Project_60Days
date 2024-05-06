using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IconMap : IconBase
{
    [SerializeField] Sprite city;    
    [SerializeField] Sprite desert;
    [SerializeField] Sprite jungle;
    [SerializeField] Sprite tundra;

    private Image image;
    private MapManager Map;

    private TileType type;

    protected override void Start()
    {
        base.Start();

        Map = App.Manager.Map;
        image = GetComponent<Image>();
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        yield return new WaitUntil(() => App.Manager.Map.tileCtrl != null);

        ResetIcon();
    }

    public void ResetIcon()
    {
        var tile = Map.tileCtrl;
        type = tile.GetComponent<TileBase>().GetTileType();

        image.sprite = SetImage();
        text = SetString();
    }

    protected override string SetString() => type switch
    {
        TileType.City => App.Data.Game.GetString("STR_TILE_NONE_DESC"),
        TileType.Desert => App.Data.Game.GetString("STR_TILE_DESERT_DESC"),
        TileType.Jungle => App.Data.Game.GetString("STR_TILE_JUNGLE_DESC"),
        TileType.Tundra => App.Data.Game.GetString("STR_TILE_TUNDRA_DESC"),
        _ => "",
    };

    private Sprite SetImage() => type switch
    {
        TileType.City => city,
        TileType.Desert => desert,
        TileType.Jungle => jungle,
        TileType.Tundra => tundra,
        _ => null,
    };
}
