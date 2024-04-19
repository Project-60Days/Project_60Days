using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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
            item.data = itemData[item.English];
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
