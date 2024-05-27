using UnityEngine;

public class TileTundra : TileBase
{
    public override TileType GetTileType() => TileType.Tundra;

    protected override void Buff() { }

    protected override void DeBuff()
    {
        int random = Random.Range(0, 100);

        if (random < 10)
        {
            App.Manager.UI.GetPanel<PagePanel>().SetResultPage("ACIDENT_ETHER", false);
        }
    }
}