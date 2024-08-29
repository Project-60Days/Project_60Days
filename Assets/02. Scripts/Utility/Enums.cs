public enum SceneName
{
    Developer,
    Title,
    Shelter,
    Map,
    UI,
    Craft
}

public enum BasicItem
{
    Steel,
    Carbon,
    Plasma,
    Powder,
    Gas,
    Rubber,
    Wire
}

#region MAP
public enum DroneType
{
    Disruptor,
    Explorer
}

public enum TileType
{
    City,
    Desert,
    Jungle,
    Tundra,
    None,
}

public enum TileState
{
    None,
    Moveable,
    Unable
}
#endregion

#region UI
public enum UIState
{
    Normal, 
    Map,
    Note,
    Bench,
    Select,
    PV,
    PopUp,
    Loading,
    Menu,
    NewDay
}

public enum SlotType
{
    InventorySlot,
    CraftingSlot,
    ResultSlot,
    EquipSlot,
    BlueprintSlot,
}

public enum QuestType
{
    Tutorial,
    Main,
    Sub
}

public enum AlertType
{
    Note,
    Caution
}

public enum BenchType
{
    Craft,
    Equip,
    Blueprint
}
#endregion