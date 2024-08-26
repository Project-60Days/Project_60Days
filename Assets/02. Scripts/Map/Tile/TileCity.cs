using UnityEngine;

public class TileCity : TileBase
{
    public override TileType GetTileType() => TileType.City;

    protected override void Buff() // Paved Road: Add 1 move range.
    {
        App.Data.Test.AddMoveRange(1);
    }

    protected override void DeBuff()
    {
        int random = Random.Range(0, 100);

        if (random < 30) // Collapsed building accident: durability reduced by -3
        {
            App.Manager.Game.ChangeDurbility(-3);
        }

        if (random < 5) // Stolen Resources: One of resources is taken away at random.
        {
            App.Manager.UI.GetPanel<InventoryPanel>().RemoveRandomItem();
        }
    }
}
