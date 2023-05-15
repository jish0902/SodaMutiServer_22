using System;
using System.Collections.Generic;
using Google.Protobuf.Protocol;

namespace Server.Data;

#region Stat

/*message StatInfo{
int32 class = 1;
int32 level = 2;
int32 hp = 3;
int32 maxHp = 4;
int32 mp = 5;
int32 maxMp = 6;
int32 attackRange = 7;
int32 attack = 8;
int32 defence = 9;
int32 critical = 10;
int32 exp = 11;
int32 faith = 12;
int32 will = 13;
int32 friendly = 14;
int32 karma = 15;
int32 frame = 16;
int32 credit = 17;
float speed = 18;
int32 totalExp = 19;
}*/

[Serializable]
public class StatData : ILoader<int, StatInfo>
{
    public List<StatInfo> stats = new();

    public Dictionary<int, StatInfo> MakeDict()
    {
        var dict = new Dictionary<int, StatInfo>();
        foreach (var stat in stats)
        {
            stat.Hp = stat.MaxHp;
            stat.Mp = stat.MaxMp;
            stat.Speed = 3; //속도
            stat.Exp = 0;
            dict.Add(100 * stat.Class + stat.Level, stat); //101
        }

        return dict;
    }
}

#endregion

#region Skill

[Serializable]
public class Skill
{
    public int amount;
    public float attackbuff; //퍼센트
    public float castTime;
    public float cooldown;
    public int cost;
    public Creature creature;
    public int damage;
    public float defencebuff;
    public string description;
    public float duration;
    public int hp;
    public int id;
    public float level_add;
    public int maxLevel;
    public string name;
    public ProjectileInfo projectile;
    public int skilltype;
}

public class Creature
{
    public int attack;
    public string attackRange; //1이면 근접
    public float attackSpeed; //초당 공격속도
    public int defence;
    public int exp;
    public int hp;
    public int mp;
    public string name;
    public float speed;
}

public class ProjectileInfo
{
    public string name;
    public string prefab;
    public int range;
    public float speed;
}

[Serializable]
public class SkillData : ILoader<int, Skill>
{
    public List<Skill> skills = new();

    public Dictionary<int, Skill> MakeDict()
    {
        var dict = new Dictionary<int, Skill>();
        foreach (var skill in skills)
            dict.Add(skill.id, skill);
        return dict;
    }
}

#endregion

//#region Item
//[Serializable]
//public class ItemData
//{
//	public int id;
//	public string name;
//	public ItemType itemType;

//}

//public class WeaponData : ItemData
//{
//	public WeaponType weaponType;
//	public int damage;

//}

//public class ArmorData : ItemData
//{
//	public ArmorType armorType;
//	public int defence;
//}

//public class ConsumableData : ItemData
//{
//	public ConsumableType consumableType;
//	public int MaxCount;
//}

//[Serializable]
//public class ItemLoader : ILoader<int, ItemData>
//{
//	public List<WeaponData> weapons = new List<WeaponData>();
//	public List<ArmorData> armors = new List<ArmorData>();
//	public List<ConsumableData> consumables = new List<ConsumableData>();

//	public Dictionary<int, ItemData> MakeDict()
//	{

//		Dictionary<int, ItemData> dict = new Dictionary<int, ItemData>();

//		foreach (ItemData item in weapons)
//		{
//			item.itemType = ItemType.Weapon;
//			dict.Add(item.id, item);
//		}
//		foreach (ItemData item in armors)
//		{
//			item.itemType = ItemType.Armor;
//			dict.Add(item.id, item);
//		}
//		foreach (ItemData item in consumables)
//		{
//			item.itemType = ItemType.Consumable;
//			dict.Add(item.id, item);
//		}

//		return dict;
//	}
//}

//#endregion

#region Mosnter

[Serializable]
public class RewardData
{
    public int count;
    public int itemId;
    public int probability; // 100분율
}

public class MonsterStat
{
    public int attack;
    public float attackRange;
    public float attackSpeed;
    public int Class;
    public int credit;
    public int critical;
    public int defence;
    public int exp;
    public int faith;
    public int frame;
    public int friendly;
    public int id;
    public int karma;
    public int level;
    public int maxExp;
    public int maxHp;
    public int maxMp;
    public int speed;
    public int will;
}

public class MonsterStatData : ILoader<int, MonsterStat>
{
    public List<MonsterStat> MonsterStat = new();

    public Dictionary<int, MonsterStat> MakeDict()
    {
        var dict = new Dictionary<int, MonsterStat>();
        foreach (var stat in MonsterStat) dict.Add(stat.id, stat);
        return dict;
    }
}

[Serializable]
public class MonsterData
{
    public int id;
    public string name;
    public string prefabPath;
    public List<RewardData> rewards;
    public StatInfo stat; //class = 100이상
}

[Serializable]
public class MonsterLoader : ILoader<int, MonsterData>
{
    public List<MonsterData> monsters = new();

    public Dictionary<int, MonsterData> MakeDict()
    {
        var dict = new Dictionary<int, MonsterData>();

        var StatData = DataManager.MonsterStatDict;

        foreach (var monster in monsters)
        {
            //---------------------------------
            MonsterStat mStat;
            if (false == StatData.TryGetValue(monster.id, out mStat))
            {
                Console.WriteLine("몬스터 스텟 병합오류");
                continue;
            }

            monster.stat = new StatInfo();

            monster.stat.Class = mStat.Class;
            monster.stat.Level = mStat.level;
            monster.stat.MaxHp = mStat.maxHp;
            monster.stat.MaxMp = mStat.maxMp;
            monster.stat.AttackRange = mStat.attackRange;
            monster.stat.Attack = mStat.attack;
            monster.stat.AttackSpeed = mStat.attackSpeed;
            monster.stat.Defence = mStat.defence;
            monster.stat.Critical = mStat.critical;
            monster.stat.Exp = mStat.exp * 10; //Todo: 삭제

            monster.stat.Faith = mStat.faith;
            monster.stat.Will = mStat.will;
            monster.stat.Friendly = mStat.friendly;
            monster.stat.Karma = mStat.karma;
            monster.stat.Frame = mStat.frame;
            monster.stat.Credit = mStat.credit;
            monster.stat.Speed = mStat.speed;
            monster.stat.MaxExp = mStat.maxExp;


            //---------------------------------

            monster.stat.Hp = monster.stat.MaxHp;
            monster.stat.Mp = monster.stat.MaxMp;

            dict.Add(monster.id, monster);
        }

        return dict;
    }
}

#endregion