public enum SceneName
{
    Developer,
    Title,
    Shelter,
    Map,
    UI,
    Craft
}

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

public enum ItemType
{
    Material,
    Equipment,
    Special
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

public enum SoundType
{
    SFX,
    BGM,
    ALL
}

#region HexTile

public enum ETileType
{
    City,
    Desert,
    Jungle,
    Tundra,
    Neo
}

public enum EResourceType
{
    Steel,
    Carbon,
    Plasma,
    Powder,
    Gas,
    Rubber,
    Wire
}

public enum ETileState
{
    None,
    Moveable,
    Unable,
    Target
}

public enum EMabPrefab
{
    Player,
    Zombie,
    Disturbtor,
    Explorer,
    Tower,
    Production,
    Army
}

public enum EStructure
{
    Tower,
    Production,
    Army,
    Dynamo
}

public enum EObjectSpawnType
{
    ExcludePlayer,
    IncludePlayer,
    ExcludeEntites,
    IncludeEntites
}

public enum ETileMouseState
{
    Nothing,
    CanClick,
    CanPlayerMove,
    DronePrepared
}

#endregion