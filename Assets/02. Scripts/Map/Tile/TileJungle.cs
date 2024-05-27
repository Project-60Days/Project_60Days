using UnityEngine;

public class TileJungle : TileBase
{
    public override TileType GetTileType() => TileType.Jungle;

    protected override void Buff() 
    {
        App.Manager.Map.SetResourceCount(1);
    }

    protected override void DeBuff()
    {
        int random = Random.Range(0, 100);

        if (random < 30) 
        {
            App.Manager.UI.GetPanel<PagePanel>().SetResultPage("LOOSE_WAY", false);
            App.Manager.Map.SetRandomTile();
        }

        if (random < 10)
        {
            App.Manager.UI.GetPanel<PagePanel>().SetResultPage("SWAMP", false);
            App.Manager.Map.SetMoveRange(0);
        }
    }
}