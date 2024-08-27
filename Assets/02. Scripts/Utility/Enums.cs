public enum SceneName
{
    Developer,
    Title,
    Shelter,
    Map,
    UI,
    Craft
}

public enum SoundType
{
    SFX,
    BGM,
    ALL
}

public enum ItemType
{
    Material,
    Equipment,
    Special
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
    None,
    City,
    Desert,
    Jungle,
    Tundra,
    Neo
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
    Craft,
    Select,
    PV,
    PopUp,
    Loading,
    Menu,
    NewDay
}

public enum PageType
{
    Result,
    Select
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

public enum CraftMode
{
    Craft,
    Equip,
    Blueprint
}
#endregion