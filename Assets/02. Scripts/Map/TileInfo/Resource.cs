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

    public Resource(ItemBase _itemBase, int _itemCount)
    {
        Item = _itemBase;
        Count = _itemCount;
    }

    public Resource(string _itemCode, int _itemCount)
    {
        Item = App.Manager.Game.itemSO.items.ToList().Find(x => x.data.Code == _itemCode);
        Count = _itemCount;
    }
}