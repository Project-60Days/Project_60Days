using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager : ManagementBase
{
    [SerializeField] public ItemSO itemSO;

    public Dictionary<string, StringData> stringData = new Dictionary<string, StringData>();
    public Dictionary<string, GameData> gameData = new Dictionary<string, GameData>();
    public Dictionary<string, ItemData> itemData = new Dictionary<string, ItemData>();
    public Dictionary<int, ItemCombineData> itemCombineData = new Dictionary<int, ItemCombineData>();
    public Dictionary<int, TileData> tileData = new Dictionary<int, TileData>();
    public Dictionary<string, DiaryData> diaryData = new Dictionary<string, DiaryData>();
    public Dictionary<string, StructData> structData = new Dictionary<string, StructData>();

    private void Awake()
    {
        LoadData();
        InitItemSO();
    }

    public void LoadData()
    {
        stringData.Clear();
        gameData.Clear();
        itemData.Clear();
        itemCombineData.Clear();
        tileData.Clear();

        var stringDataRaw = DataLoader.LoadData<StringData>(StringUtility.stringDataPath);
        var gameDataRaw = DataLoader.LoadData<GameData>(StringUtility.gameDataPath);
        var itemDataRaw = DataLoader.LoadData<ItemData>(StringUtility.itemDataPath);
        var itemCombineDataRaw = DataLoader.LoadData<ItemCombineData>(StringUtility.itemCombineDataPath);
        var tileDataRaw = DataLoader.LoadData<TileData>(StringUtility.tileDataPath);

        foreach (var data in stringDataRaw)
            stringData.Add(data.Code, data);

        foreach (var data in gameDataRaw)
            gameData.Add(data.Code, data);

        foreach (var data in itemDataRaw)
            itemData.Add(data.Code, data);

        foreach (var data in itemCombineDataRaw)
            itemCombineData.Add(data.Index, data);

        foreach (var data in tileDataRaw)
            tileData.Add(data.Index, data);
    }

    private void InitItemSO()
    {
        foreach(var item in itemSO.items)
        {
            item.data = itemData[item.itemCode];
        }
    }

    public string GetString(string _code)
    {
        string language = "";

        if (!stringData.ContainsKey(_code))
            return "String is Not Exist";

        switch (Application.systemLanguage)
        {
            case SystemLanguage.Korean:
                language = stringData[_code].Korean;
                break;

            case SystemLanguage.Chinese:
                language = stringData[_code].Chinese;
                break;

            case SystemLanguage.Japanese:
                language = stringData[_code].Japanese;
                break;

            default:
                language = stringData[_code].English;
                break;
        }

        return language;
    }

    public override EManagerType GetManagemetType()
    {
        return EManagerType.DATA;
    }
}

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
public class GameData
{
    public int Index;
    public string Code;
    public float value;
}

[Serializable]
public class ItemData
{
    public int Index;
    public int Category;
    public int Tier;
    public string Code;
    public int Rarity;
    public int AppearTileType;
    public float TileIncludePossibility;
    public int Type;
    public int EquipType;
    public int isCanBase;
    public int CombineSlotCount;
    public string Description;
    public string Korean;
    public string English;
    public string Japanese;
    public string Chinese;
}

[Serializable]
public class ItemCombineData
{
    public int Index;
    public string Material_1;
    public string Material_2;
    public string Material_3;
    public string Material_4;
    public string Material_5;
    public string Material_6;
    public string Material_7;
    public string Material_8;
    public string Result;
}

[Serializable]
public class TileData
{
    public int Index;
    public int TileTypeAppearPosibility;
    public int TileSwarmMinCount;
    public int TileSwarmMaxCount;
    public int RemainPossibility_Metal;
    public int RemainPossibility_Carbon;
    public int RemainPossibility_Plasma;
    public int RemainPossibility_Pawder;
    public int RemainPossibility_Gas;
    public int RemainPossibility_Rubber;
    public string Korean;
    public string English;
    public string Japanese;
    public string Chinese;
}

[Serializable]
public class DiaryData
{
    public int Index;
    public int code;
   // public EScriptType ScriptType;
    public int Script;
    public int IsSelectScript;
    public int RemainPossibility;
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