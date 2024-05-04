using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IconMap : IconBase
{
    [SerializeField] Sprite city;    
    [SerializeField] Sprite desert;
    [SerializeField] Sprite jungle;
    [SerializeField] Sprite tundra;

    Image image;
    Player player;

    ETileType type;

    protected override void Start()
    {
        base.Start();

        image = GetComponent<Image>();
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        yield return new WaitUntil(() => App.Manager.Map.mapCtrl.Player != null);

        player = App.Manager.Map.mapCtrl.Player;
        ReInit();
    }

    public void ReInit()
    {
        SetImage();
        text = SetString();
    }

    protected override string SetString() => type switch
    {
        ETileType.None => App.Data.Game.GetString("STR_TILE_NONE_DESC"),
        ETileType.Desert => App.Data.Game.GetString("STR_TILE_DESERT_DESC"),
        ETileType.Jungle => App.Data.Game.GetString("STR_TILE_JUNGLE_DESC"),
        ETileType.Tundra => App.Data.Game.GetString("STR_TILE_TUNDRA_DESC"),
        ETileType.Neo => App.Data.Game.GetString("STR_TILE_NEO_DESC"),
        _ => "",
    };

    public void SetImage()
    {
        var tile = player.TileController;
        type = tile.GetComponent<TileBase>().TileType;

        switch (type)
        {
            case ETileType.None:
                image.sprite = city;
                break;
            case ETileType.Jungle:
                image.sprite = jungle;
                break;
            case ETileType.Desert:
                image.sprite = desert;
                break;
            case ETileType.Tundra:
                image.sprite = tundra;
                break;
        }
    }
}
