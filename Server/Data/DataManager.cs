using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Google.Protobuf.Protocol;
using Newtonsoft.Json;

namespace Server.Data;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    private static string _file;
    public static Dictionary<int, StatInfo> StatDict { get; private set; } = new();

    public static Dictionary<int, Skill> SkillDict { get; private set; } = new();

    //public static Dictionary<int, Data.ItemData> ItemDict { get; private set; } = new Dictionary<int, Data.ItemData>();
    public static Dictionary<int, MonsterStat> MonsterStatDict { get; private set; } = new();

    public static Dictionary<int, MonsterData> MonsterDict { get; private set; } = new();


    public static void LoadData(string file = null)
    {
        _file = file;
        StatDict = LoadJson<StatData, int, StatInfo>("StatData").MakeDict();
        SkillDict = LoadJson<SkillData, int, Skill>("SkillData").MakeDict();
        //ItemDict = LoadJson<Data.ItemLoader, int, Data.ItemData>("ItemData").MakeDict();
        MonsterStatDict = LoadJson<MonsterStatData, int, MonsterStat>("MonsterStatData").MakeDict();
        MonsterDict = LoadJson<MonsterLoader, int, MonsterData>("MonsterData").MakeDict();
    }

    private static Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        string text;
        if (_file == null) //기본
            text = File.ReadAllText($"{ConfingManager.config.dataPath}/{path}.json");
        else
            text = File.ReadAllText(_file);

        var settings = new JsonSerializerSettings
        {
            Culture = new CultureInfo("en-US"),
            NullValueHandling = NullValueHandling.Ignore
        };
        return JsonConvert.DeserializeObject<Loader>(text, settings);
    }
}