public class TileTundra : TileBase
{
    public override TileType GetTileType() => TileType.Tundra;

    public override void Buff() { }

    public override void DeBuff()
    {
        App.Manager.UI.GetPanel<PagePanel>().SetResultPage("SEARCH_TUNDRA", false);

        if (RandomPercent.GetRandom(10))
        {
            App.Manager.UI.GetPanel<PagePanel>().SetResultPage("ACIDENT_ETHER", false);
        }

        App.Manager.Map.SetMoveRange(0);
    }
}