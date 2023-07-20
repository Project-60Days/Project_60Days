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

public enum ItemType
{
    Material,
    Equipment,
    Consumption
}

public enum ENotePageType
{
    DayStart,
    SelectEvent,
    Resource,
    Equipment,
    SpecialEvent,
    Map
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

public enum TerrainType 
{ 
    Grassland, 
    Machine, 
    Desert, 
    Water 
};
public enum TileHeight 
{ 
    Flat, 
    Uphill, 
    Downhill 
};

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
    배터리,
    강철,
    나무,
    기름,
}

#endregion