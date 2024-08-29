using UnityEngine;

public class TileJungle : TileBase
{
    public override TileType GetTileType() => TileType.Jungle;

    protected override void Buff() 
    {
        App.Data.Test.SetResourceCount(1);
    }

    protected override void DeBuff()
    {
        int random = Random.Range(0, 100);

        if (random < 30) 
        {
            App.Manager.UI.GetPanel<PagePanel>().SetNextPage(PageType.Result, "STR_RESULT_LOOSE_WAY");
            App.Manager.Map.SetRandomTile();
        }

        if (random < 10)
        {
            App.Manager.UI.GetPanel<PagePanel>().SetNextPage(PageType.Result, "STR_RESULT_SWAMP");
            App.Data.Test.SetMoveRange(0);
        }
    }
}