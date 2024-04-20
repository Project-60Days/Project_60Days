public enum SceneName
{
    Developer,
    Title,
    Game,
    UI,
    Map,
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
    Menu
}

public enum EItemType
{
    Material,
    Equipment,
    Special
}

public enum ENotePageType
{
    Result,
    Select
}

public enum ESlotType
{
    InventorySlot,
    CraftingSlot,
    ResultSlot,
    EquipSlot,
    BlueprintSlot,
}

public enum EQuestType
{
    Tutorial,
    Main,
    Sub
}

public enum EAlertType
{
    Selection,
    Caution
}

public enum CraftMode
{
    Craft,
    Equip,
    Blueprint
}

public enum EScriptType
{
    Diary,
    Resource,
    MainQuest,
    SubQuest
}

public enum ESoundType
{
    SFX,
    BGM,
    ALL
}

#region HexTile

public enum ETileType
{
    None,
    Jungle,
    Desert,
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