using UnityEngine;

public class TileDesert : TileBase
{
    public override TileType GetTileType() => TileType.Desert;

    protected override void Buff() { }

    protected override void DeBuff()
    {
        int random = Random.Range(0, 100);

        App.Manager.Game.Durability -= -1;

        if (random < 10) // Sandstorm debuff: unable to move for a day
        {
            App.Manager.UI.GetPanel<PagePanel>().SetNextPage(PageType.Result, "STR_RESULT_DESERT_STORM");
            App.Data.Test.SetMoveRange(0);
        }
    }
}
