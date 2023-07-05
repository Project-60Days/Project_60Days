using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager : Singleton<DataManager>
{
    public Dictionary<string, StringData> stringData = new Dictionary<string, StringData>();
    public Dictionary<string, CharData> charData = new Dictionary<string, CharData>();
    public Dictionary<string, GameData> gameData = new Dictionary<string, GameData>();
    //public Dictionary<string, ItemData> itemData = new Dictionary<string, ItemData>();

    //public Dictionary<int, Stat> StatDict { get; private set; } = new Dictionary<int, Stat>();

    //public void InitDict()
    //{
    //    StatDict = LoadJson<StatData, int, Stat>("StatData").MakeDict();
    //}

    //Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    //{
    //    TextAsset textAsset = Resources.Load<TextAsset>($"Data/{path}"); // text 파일이 textAsset에 담긴다. TextAsset 타입은 텍스트파일 에셋이라고 생각하면 됨!
    //    return JsonUtility.FromJson<Loader>(textAsset.text);
    //}

    private void Awake()
    {
        InitStringData();
        InitGameData();
        //InitDict();
    }

    public void InitGameData()
    {
        charData.Clear();
        gameData.Clear();

        var jsonGameData = Resources.Load<TextAsset>(StringUtility.gameDataPath);
        var stringDataList = JsonUtilityHelper.FromJson<GameData>(jsonGameData.ToString());

        foreach(var data in stringDataList)
        {
            gameData.Add(data.Code, data);
        }

    }

    public void InitStringData()
    {
        stringData.Clear();

        var json = Resources.Load<TextAsset>(StringUtility.stringDataPath);
        var stringDataList = JsonUtilityHelper.FromJson<StringData>(json.ToString());

        foreach(var data in stringDataList)
        {
            stringData.Add(data.Code, data);
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
public class CharData
{
    public int index;
    public string code;
    public string nameCode;
    public string storyCode;
    public int minHP;
    public int maxHp;
    public int minFood;
    public int maxFood;
    public int minWater;
    public int maxWater;
    public int minBattery;
    public int maxBattery;
    public int dayFoodCost;
    public int dayWaterCost;
    public EBodyHealthType health;
    public int immunity;
    public int mental;
    public int mentalEffectType;
    public string equipmentCode;
    public int power;
    public string buttCode;
    public string relationshipCode1;
    public string relationshipCode2;
}

//public class ItemData
//{
//    public int ID;
//    public string Name;
//    public string ToolTip;
//    public Sprite IconSprite;
//    public int maxAmount;
//    public int minAmount;

//}

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