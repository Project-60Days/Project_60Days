using UnityEngine;

public class TileDesert : TileBase
{
    public override TileType GetTileType() => TileType.Desert;

    public override void Buff() { }

    public override void DeBuff()
    {
        int random = Random.Range(0, 100);

        App.Manager.Game.ChangeDurbility(-1);

        if (random < 10) // Sandstorm debuff: unable to move for a day
        {
            App.Manager.UI.GetPanel<PagePanel>().SetResultPage("DESERT_STORM",false);
            App.Manager.Map.SetMoveRange(0);
        }
    }
}
