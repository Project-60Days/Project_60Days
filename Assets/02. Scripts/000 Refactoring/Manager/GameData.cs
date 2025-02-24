using System;
using System.Collections.Generic;
using UnityEngine;

#region Data Class
[Serializable]
public class StringData
{
    public int Index;
    public string Code;
    public string Korean;
    public string English;
    public string Japanese;
    public string Chinese;
}

[Serializable]
public class ValueData
{
    public string Code;
    public float Move;
    public float Stop;
    public float SpecialSpawn;
    public float MinCount;
    public float MaxCount;
}

[Serializable]
public class TileData
{
    public string Code;
    public int TileTypeAppearPosibility;
    public int TileSwarmMinCount;
    public int TileSwarmMaxCount;
    public int RemainPossibility_Steel;
    public int RemainPossibility_Carbon;
    public int RemainPossibility_Plasma;
    public int RemainPossibility_Powder;
    public int RemainPossibility_Gas;
    public int RemainPossibility_Rubber;
    public string Korean;
    public string English;
    public string Japanese;
    public string Chinese;
}

[Serializable]
public class StructData
{
    public int Index;
    public string Code;
    public string Korean;
    public string English;
    public string Japanese;
    public string Chinese;
    public string Item;
    public int Count;
    public string SpecialItem;
}

[Serializable]
public class ItemData
{
    public int Index;
    public int Category;
    public string Code;
    public int AppearTileType;
    public int Type;
    public int EquipType;
    public string EquipArea;
    public int CombineSlotCount;
    public string Description;
    public string EffectDescription;
    public string Korean;
    public string English;
    public string Japanese;
    public string Chinese;
    public float value1;
    public float value2;
    public float value3;
}

[Serializable]
public class ItemCombineData
{
    public int Index;
    public string Material_1;
    public string Material_2;
    public string Material_3;
    public string Result;
}
#endregion

#region Data Loader
public class DataLoader
{
    public static T[] LoadData<T>(string dataPath)
    {
        var json = Resources.Load<TextAsset>(dataPath);

        if (json)
        {
            var stringDataList = JsonUtilityHelper.FromJson<T>(json.ToString());

            return stringDataList;
        }

        return null;
    }
}

public class JsonUtilityHelper
{
    public static T[] FromJson<T>(string json)
    {
        string newJson = "";
        if (json[0] == '{')
        {
            newJson = json;
        }
        else
        {
            newJson = "{ \"array\": " + json + "}";
        }
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.array = array;
        return JsonUtility.ToJson(wrapper);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}
#endregion

public class GameData : Data
{
    public Dictionary<string, StringData> stringData = new Dictionary<string, StringData>();

    public Dictionary<string, ValueData> valueData = new Dictionary<string, ValueData>();
    public Dictionary<string, TileData> tileData = new Dictionary<string, TileData>();
    public Dictionary<string, StructData> structData = new Dictionary<string, StructData>();

    public Dictionary<string, ItemData> itemData = new Dictionary<string, ItemData>();
    public Dictionary<int, ItemCombineData> itemCombineData = new Dictionary<int, ItemCombineData>();

    #region Data Path
    private string stringDataPath = "Data/StringData";
    private string valueDataPath = "Data/ValueData";
    private string tileDataPath = "Data/TileData";
    private string structDataPath = "Data/StructData";
    private string itemDataPath = "Data/ItemData";
    private string itemCombineDataPath = "Data/ItemCombineData";
    #endregion

    protected override void Awake()
    {
        base.Awake();

        LoadData();
    }

    public void LoadData()
    {
        stringData.Clear();
        valueData.Clear();
        tileData.Clear();
        structData.Clear();
        itemData.Clear();
        itemCombineData.Clear();

        var stringDataRaw = DataLoader.LoadData<StringData>(stringDataPath);
        var valueDataRaw = DataLoader.LoadData<ValueData>(valueDataPath);
        var tileDataRaw = DataLoader.LoadData<TileData>(tileDataPath);
        var structDataRaw = DataLoader.LoadData<StructData>(structDataPath);
        var itemDataRaw = DataLoader.LoadData<ItemData>(itemDataPath);
        var itemCombineDataRaw = DataLoader.LoadData<ItemCombineData>(itemCombineDataPath);

        foreach (var data in stringDataRaw)
            stringData.Add(data.Code, data);

        foreach (var data in valueDataRaw)
            valueData.Add(data.Code, data);

        foreach (var data in tileDataRaw)
            tileData.Add(data.Code, data);

        foreach (var data in structDataRaw)
            structData.Add(data.Code, data);

        foreach (var data in itemDataRaw)
            itemData.Add(data.Code, data);

        foreach (var data in itemCombineDataRaw)
            itemCombineData.Add(data.Index, data);
    }

    public string GetString(string _code)
    {
        TryGetString(_code, out var str);
        return str;
    }

    public bool TryGetString(string _code, out string str)
    {
        return TryGetString(_code, App.Data.Setting.Language, out str);
    }

    public bool TryGetString(string _code, SystemLanguage _language, out string str)
    {
        if (_code != null && stringData.TryGetValue(_code, out var data))
        {
            str = _language switch
            {
                SystemLanguage.Korean => data.Korean,
                SystemLanguage.Chinese => data.Chinese,
                SystemLanguage.Japanese => data.Japanese,
                _ => data.English
            };

            return true;
        }

        str = "String not found";
        return false;
    }
}
