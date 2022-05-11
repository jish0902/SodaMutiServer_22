﻿using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.Protocol;

namespace Server.Data
{

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
		public List<StatInfo> stats = new List<StatInfo>();

		public Dictionary<int, StatInfo> MakeDict()
		{
			Dictionary<int, StatInfo> dict = new Dictionary<int, StatInfo>();
			foreach (StatInfo stat in stats)
			{
				stat.Hp = stat.MaxHp;
				stat.Mp = stat.MaxMp;
				stat.Speed = 3; //속도
				stat.TotalExp = stat.Exp;
				stat.Exp = 0;
				dict.Add(1000 * stat.Class + stat.Level, stat); //1001
				
			}
			return dict;
		}
	}
	#endregion

	#region Skill
	[Serializable]
	public class Skill
	{
		public int id;
		public int maxLevel;
		public string name;
		public float level_add;
		public string description;
        public float castTime;
        public float duration;
        public float cooldown;
		public int skilltype;
        public int cost;
        public int amount;
        public float attackbuff; //퍼센트
		public float defencebuff;
		public int damage;
		public int hp;
		public ProjectileInfo projectile;
		public Creature creature;
	}

    public class Creature
    {
        public string name;
        public int attack;
        public float attackSpeed; //초당 공격속도
        public string attackRange; //1이면 근접
        public int hp;
        public int mp;
        public float speed;
        public int defence;
        public int exp;
        
    }

	public class ProjectileInfo
	{
		public string name;
		public float speed;
		public int range;
		public string prefab;
	}


	[Serializable]
	public class SkillData : ILoader<int, Skill>
	{
		public List<Skill> skills = new List<Skill>();

		public Dictionary<int, Skill> MakeDict()
		{
			Dictionary<int, Skill> dict = new Dictionary<int, Skill>();
			foreach (Skill skill in skills)
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
        public int probability; // 100분율
        public int itemId;
        public int count;
    }

    [Serializable]
    public class MonsterData
    {
        public int id;
        public string name;
        public StatInfo stat;  //class = 100이상
        public List<RewardData> rewards;
        public string prefabPath;

    }

    [Serializable]
    public class MonsterLoader : ILoader<int, MonsterData>
    {
        public List<MonsterData> monsters = new List<MonsterData>();
        public Dictionary<int, MonsterData> MakeDict()
        {
            Dictionary<int, MonsterData> dict = new Dictionary<int, MonsterData>();
            foreach (MonsterData monster in monsters)
            {
                monster.stat.Hp = monster.stat.MaxHp;
                monster.stat.Mp = monster.stat.MaxMp;
                monster.stat.Speed = 3; //속도
                monster.stat.TotalExp = monster.stat.Exp;
                monster.stat.Exp = 0;

                dict.Add(monster.id, monster);
            }
            return dict;
        }
    }


    #endregion




}
