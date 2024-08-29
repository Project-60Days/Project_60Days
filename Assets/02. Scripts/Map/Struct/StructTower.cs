using Hexamap;
using System.Collections.Generic;

public class StructTower : StructBase
{
    protected override string GetCode() => "STRUCT_TOWER";

    private InventoryPanel inventory;

    public override void Init(List<Tile> _colleagueList)
    {
        base.Init(_colleagueList);

        inventory = App.Manager.UI.GetPanel<InventoryPanel>();
    }

    public override void DetectStruct()
    {
        if (inventory.CheckItemExist("ITEM_NETWORKCHIP"))
        {
            base.DetectStruct();
            inventory.RemoveItemByCode("ITEM_NETWORKCHIP");
        }
    }
}
