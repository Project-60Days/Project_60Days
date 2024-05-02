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
    public int Index;
    public string Code;
    public float value;
}

[Serializable]
public class TileData
{
    public int Index;
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
    public string code;
    public bool IsCanAccess;
    public string YesFuncName;
    public string NoFuncName;
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
    public Dictionary<int, TileData> tileData = new Dictionary<int, TileData>();
    public Dictionary<string, StructData> structData = new Dictionary<string, StructData>();

    public Dictionary<string, ItemData> itemData = new Dictionary<string, ItemData>();
    public Dictionary<int, ItemCombineData> itemCombineData = new Dictionary<int, ItemCombineData>();

    #region Data Path
    string stringDataPath = "Data/StringData";
    string valueDataPath = "Data/ValueData";
    string itemDataPath = "Data/ItemData";
    string itemCombineDataPath = "Data/ItemCombineData";
    string tileDataPath = "Data/TileData";
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
        itemData.Clear();
        itemCombineData.Clear();
        tileData.Clear();

        var stringDataRaw = DataLoader.LoadData<StringData>(stringDataPath);
        var valueDataRaw = DataLoader.LoadData<ValueData>(valueDataPath);
        var itemDataRaw = DataLoader.LoadData<ItemData>(itemDataPath);
        var itemCombineDataRaw = DataLoader.LoadData<ItemCombineData>(itemCombineDataPath);
        var tileDataRaw = DataLoader.LoadData<TileData>(tileDataPath);

        foreach (var data in stringDataRaw)
            stringData.Add(data.Code, data);

        foreach (var data in valueDataRaw)
            valueData.Add(data.Code, data);

        foreach (var data in itemDataRaw)
            itemData.Add(data.Code, data);

        foreach (var data in itemCombineDataRaw)
            itemCombineData.Add(data.Index, data);

        foreach (var data in tileDataRaw)
            tileData.Add(data.Index, data);
    }

    public string GetString(string _code)
    {
        TryGetString(_code, out var str);
        return str;
    }

    public bool TryGetString(string _code, out string str)
    {
        return TryGetString(_code, SystemLanguage.Korean, out str); //TODO: Set language options and enter parameters accordingly
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
