using Server.Data;
using Server.Game;
using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.Protocol;
using Google.Protobuf;
using Server.Game.Room;

public class SkillHandler
{
    /*
      Skill _skill;
        if (DataManager.SkillDict.TryGetValue(, out _skill) == false)
            return;

        //if(obj.stat.Mp <= 0 || obj.stat.Mp < _skill.)
        #region 검사구역

        if (obj.ObjectType != GameObjectType.Player) //플레이어라면
            return;

        //----------------- 쿨타임 ---------------
        Player p = (Player)obj;
        bool t = p.ApplySkill(, _skill.cooldown);
        Console.WriteLine(t ? $"{obj.info.Name}성공" : $"{obj.info.Name}실패");
        if (t == false) //실패하면
            return;


        //----------------- 코스트 ---------------







        #endregion
        //------------ 통과 --------------
       

        //---------------- 후처리 --------------
    

       //------------ 
       Console.WriteLine("Skill____________");
     */
    internal static void Skill100(GameObject obj) //성기사 기본 공격
    {
        //스킬 찾기
        Skill _skill;
        if (DataManager.SkillDict.TryGetValue(100, out _skill) == false)
            return;

        //if(obj.stat.Mp <= 0 || obj.stat.Mp < _skill.)
        #region 검사구역

        if (obj.ObjectType != GameObjectType.Player) //플레이어라면
            return;

        //----------------- 쿨타임 ---------------
        Player p = (Player)obj;
        bool t = p.ApplySkill(100, _skill.cooldown);
        Console.WriteLine(t ? $"{obj.info.Name}성공" : $"{obj.info.Name}실패");
        if (t == false) //실패하면
            return;


        //----------------- 코스트 ---------------







        #endregion
        //------------ 통과 --------------
        if(p.Targets == null|| p.Targets.Count == 0)
            return;

        foreach (GameObject go in p.Targets)
        {
            if(go != null || go.Room != null || p.Room != null|| go.Room == p.Room || go.State != CreatureState.Dead)
                go.OnDamaged(p, _skill.damage + p.Attack);
        }

        //---------------- 후처리 --------------


        //------------ 
        Console.WriteLine("Skill100____________");
    }

    internal static void Skill101(GameObject obj) //성기사 버프를 준다
    {
        Skill _skill;
        if (DataManager.SkillDict.TryGetValue(101, out _skill) == false)
            return;

        //if(obj.stat.Mp <= 0 || obj.stat.Mp < _skill.)
        #region 검사구역

        if (obj.ObjectType != GameObjectType.Player) //플레이어라면
            return;

        //----------------- 쿨타임 ---------------
        Player p = (Player)obj;
        bool t = p.ApplySkill(101, _skill.cooldown);
        Console.WriteLine(t ? $"{obj.info.Name}성공" : $"{obj.info.Name}실패");
        if (t == false) //실패하면
            return;


        //----------------- 코스트 ---------------







        #endregion
        //------------ 통과 --------------
        obj.stat.Attack += (int)_skill.attackbuff;
        obj.stat.Defence += (int)_skill.attackbuff;


        S_Skill skillPacket = new S_Skill() { Info = new SkillInfo() };
        skillPacket.ObjectId = obj.Id;
        skillPacket.Info.SkillId = 101;
        obj.Room.Push(obj.Room.BroadCast, obj.CurrentRoomId, skillPacket);


        S_StatChange statPacket = new S_StatChange();
        statPacket.ObjectId = obj.Id;
        statPacket.StatInfo = obj.stat;
        obj.Room.Push(obj.Room.BroadCast, obj.CurrentRoomId, statPacket);

        //---------------- 후처리 --------------
    
        obj.Room.PushAfter((int)_skill.duration * 1000,  () => {
            if (obj == null|| obj.Room == null)
                return;

            obj.stat.Attack -= (int)_skill.attackbuff;            ////Todo: 나중에 디버프 같은거 생각해서 고치기 (이게 맞나)
            obj.stat.Defence -= (int)_skill.attackbuff;
            S_StatChange StatAfter = new S_StatChange();
            statPacket.ObjectId = obj.Id;
            statPacket.StatInfo = obj.stat;
            obj.Room.Push(obj.Room.BroadCast, obj.CurrentRoomId, StatAfter);
            return; 
        });

       //------------ 
       Console.WriteLine("Skill100____________");
    }
    internal static void Skill102(GameObject obj)
    {
        throw new NotImplementedException();
    }
    internal static void Skill103(GameObject obj)
    {
        throw new NotImplementedException();
    }
    internal static void Skill104(GameObject obj)
    {
        throw new NotImplementedException();
    }

    internal static void Skill110(GameObject obj)
    {
        throw new NotImplementedException();
    }

 
   
 

  


    internal static void Skill205(GameObject obj)
    {
        throw new NotImplementedException();
    }

   
    internal static void Skill204(GameObject obj)
    {
        throw new NotImplementedException();
    }

    internal static void Skill203(GameObject obj)
    {
        throw new NotImplementedException();
    }

    internal static void Skill202(GameObject obj)
    {
        throw new NotImplementedException();
    }

    internal static void Skill201(GameObject obj)
    {
        throw new NotImplementedException();
    }

    internal static void Skill200(GameObject obj)
    {
        throw new NotImplementedException();
    }
    internal static void Skill301(GameObject obj)
    {
        throw new NotImplementedException();
    }

    internal static void Skill303(GameObject obj)
    {
        throw new NotImplementedException();
    }

    internal static void Skill304(GameObject obj)
    {
        throw new NotImplementedException();
    }

    internal static void Skill302(GameObject obj)
    {
        throw new NotImplementedException();
    }

    internal static void Skill300(GameObject obj)
    {
        throw new NotImplementedException();
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

        p.Room.Push(p.Room.BroadCast, p.CurrentRoomId, skillPacket);


        Arrow arrow = ObjectManager.Instance.Add<Arrow>();
        if (arrow == null)
            return;
        arrow.CurrentRoomId = p.CurrentRoomId;
        arrow.info.Name = "Arrow";
        arrow.Owner = p;
        arrow.Data = skill;
        arrow.PosInfo.State = CreatureState.Moving;
        arrow.PosInfo.DirX = p.SkillDir.X;
        arrow.PosInfo.DirY  = p.SkillDir.Y;
        arrow.PosInfo.PosX = p.PosInfo.PosX;
        arrow.PosInfo.PosY = p.PosInfo.PosY;
        arrow.Speed = skill.projectile.speed;

        Console.WriteLine($"P pos : {p.PosInfo.PosX},{p.PosInfo.PosY}");
        Console.WriteLine($"arrow pos : {arrow.PosInfo.PosX},{arrow.PosInfo.PosY}");
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

