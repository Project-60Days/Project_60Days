using System.Collections.Generic;
using Hexamap;

public class ResourceUnit : MapBase
{
    private List<Resource> resources;

    public override void Init() { }

    public override void ReInit()
    {
        resources = tile.GetComponent<TileBase>().GetResources();
        var Inventory = App.Manager.UI.GetPanel<InventoryPanel>();

        foreach (var resource in resources)
        {
            Inventory.AddItem(resource.Item);
        }

        App.Manager.Sound.PlaySFX(resources[0].Item.sfxName);
    }

    public bool CheckResource(TileController tileController)
    {
        return tileController.GetComponent<TileBase>().CheckResources();
    }

    public List<Resource> GetLastResources()
        => resources != null || resources.Count > 0 ? resources : null;
}