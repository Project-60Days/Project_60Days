public class TileTundra : TileBase
{
    public override void Buff(Player player) { }

    public override void DeBuff(Player _player)
    {
        App.Manager.Map.TundraTileCheck();

        if (RandomPercent.GetRandom(10))
        {
            App.Manager.Map.EtherResourceCheck();
        }
    }
}