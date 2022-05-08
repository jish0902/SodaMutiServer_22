using Server.Data;
using Server.Game;
using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.Protocol;
using Google.Protobuf;

public class SkillHandler
{

    internal static void Skill100(GameObject obj)
    {
        //스킬 찾기
        Skill _skill;
        if (DataManager.SkillDict.TryGetValue(100, out _skill) == false)
            return;


        //if(obj.stat.Mp <= 0 || obj.stat.Mp < _skill.)
        //검사
        if (obj.ObjectType != GameObjectType.Player)
            return;

        Player p = (Player)obj;
        bool t = p.ApplySkill(100,_skill.cooldown);
        Console.WriteLine(t ? $"{obj.info.Name}성공" : $"{obj.info.Name}실패");
        if (t == false) //실패하면
            return;

        //------------ 통과 --------------
        obj.stat.Attack += (int)_skill.attackbuff;
        obj.stat.Defence += (int)_skill.attackbuff;
        

        S_Skill skillPacket = new S_Skill() { Info = new SkillInfo() };
        skillPacket.ObjectId = obj.Id;
        skillPacket.Info.SkillId = 100;
        //obj.Room.Push(() => { obj.Room.BroadCast(obj.CurrentPlanetId, skillPacket); });
        obj.Room.Push(obj.Room.BroadCast, obj.CurrentPlanetId, skillPacket);


        S_StatChange statPacket = new S_StatChange();
        statPacket.ObjectId = obj.Id;
        statPacket.StatInfo = obj.stat;
        obj.Room.Push(obj.Room.BroadCast,obj.CurrentPlanetId, statPacket);


        Console.WriteLine("Skill100____________");
    }

    internal static void Skill502(GameObject obj)
    {
        throw new NotImplementedException();
    }

    internal static void Skill11(GameObject obj)
    {
        //스킬 찾기
        Skill skill = null;
        if (DataManager.SkillDict.TryGetValue(501, out skill) == false)  //Todo
            return;

        //검사
        if (obj.ObjectType != GameObjectType.Player)
            return;

        Player p = (Player)obj;
        bool t = p.ApplySkill(11, skill.cooldown);
        Console.WriteLine(t ? $"{obj.info.Name} 성공" : $"{obj.info.Name} 실패");
        if (t == false) //실패하면
            return;

        //------------ 통과 --------------

        S_Skill skillPacket = new S_Skill() { Info = new SkillInfo() };
        skillPacket.ObjectId = obj.Id;
        skillPacket.Info.SkillId = 501;

        p.Room.Push(p.Room.BroadCast, p.CurrentPlanetId, skillPacket);


        Arrow arrow = ObjectManager.Instance.Add<Arrow>();
        if (arrow == null)
            return;
        arrow.CurrentPlanetId = p.CurrentPlanetId;
        arrow.info.Name = "Arrow";
        arrow.Owner = p;
        arrow.Data = skill;
        arrow.PosInfo.State = CreatureState.Moving;
        arrow.PosInfo.DirX = p.SkillDir.X;
        arrow.PosInfo.DirY  = p.SkillDir.Y;
        arrow.PosInfo.PosX = p.PosInfo.PosX;
        arrow.PosInfo.PosY = p.PosInfo.PosY;
        Console.WriteLine($"pos : {p.PosInfo.PosX},{p.PosInfo.PosY}");
        arrow.Speed = skill.projectile.speed;
        p.Room.Push(p.Room.EnterGame, arrow, false);

        
       // Console.WriteLine("Skill11____________");
        //Console.WriteLine ($"{arrow.PosInfo.DirX},{arrow.PosInfo.DirY},{arrow.PosInfo.PosX},{arrow.PosInfo.PosY}");
    }

    internal static void Skill10(GameObject obj)
    {
        throw new NotImplementedException();
    }

    internal static void Skill500(GameObject obj)
    {
        Console.WriteLine("Skill500");

    }

    internal static void Skill501(GameObject obj)
    {
        Console.WriteLine("Skill501");

    }
}

