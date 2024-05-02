using System.Linq;

public class Resource
{
    public ItemBase Item { get; private set; }

    private int count;

    public int Count
    {
        get => count;
        set
        {
            if (value < 0)
                count = 0;
            else
                count = value;
        }
    }

    public Resource(ItemBase _item, int _count)
    {
        Item = _item;
        Count = _count;
    }

    public Resource(string _code, int _count)
    {
        Item = App.Manager.Game.itemSO.items.ToList().Find(x => x.data.Code == _code);
        Count = _count;
    }
}