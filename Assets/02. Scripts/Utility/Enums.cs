public enum ESceneType
{
    Title,
    Game,
    UI,
    Map,
    Crafting
}

public enum EUILayer
{
    Base,
    UI,
    Setting
};

public enum EItemType
{
    Material,
    Equipment,
    Consumption
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
    EquipSlot
}

public enum EQuestType
{
    Main,
    Sub
}

public enum EAlarmType
{
    New,
    Result,
    Caution
}

public enum ECraftModeType
{
    Craft,
    Equip,
    Blueprint
}

#region Character

public enum EHungerType
{
    Normal,
    Hunger,
    Starving
};
public enum EThirstType
{
    Normal,
    Thirst,
    Dehydration
};
public enum EConditionType
{
    Normal,
    Anx,
    Crazy,
    Machine
};
public enum EBodyHealthType
{
    Normal,
    Hurt,
    Injury,
    Disease
};
public enum EInfectionType
{
    Normal,
    Bite,
    Zombie
};
public enum EPartsType
{
    None,
    Eyeball,
    LeftArm,
    RightArm,
    Body,
    Legs
};

#endregion

#region HexTile

public enum ETileType
{
    Grassland,
    Machine,
    Desert,
    Water
}

public enum EWeatherType
{
    Sunny,
    Cloudy,
    Rainy,
    Fog
}

public enum EResourceType
{
    PLASTIC,
    STEEL,
    PLAZMA,
    POWDER,
    GAS,
    PLATE
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
    Explorer
}

#endregion