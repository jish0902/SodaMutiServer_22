using Server.Data;
using Server.Game;
using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.Protocol;
using Google.Protobuf;
using System.Numerics;
using System.Linq;

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
       
        //Todo : 거리 채ㅔ크






        #endregion
        //------------ 통과 --------------
        if(p.Targets == null|| p.Targets.Count == 0)
            return;

        foreach (GameObject go in p.Targets)  //아마 상관 없음
        {
            if(go != null || go.gameRoom != null || p.gameRoom != null|| go.gameRoom == p.gameRoom || go.State != CreatureState.Dead)
                go.OnDamaged(p, _skill.damage + p.Attack);
            Console.WriteLine($"{p.info.Name}이 {go.info.Name}에게 {_skill.damage + p.Attack}데미지 줌  남은 피: {go.Hp}");
        }


        //---------------- 후처리 --------------


        //------------ 
        
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
        obj.gameRoom.Push(obj.gameRoom.BroadCast, obj.CurrentRoomId, skillPacket);


        S_StatChange statPacket = new S_StatChange();
        statPacket.ObjectId = obj.Id;
        statPacket.StatInfo = obj.stat;
        obj.gameRoom.Push(obj.gameRoom.BroadCast, obj.CurrentRoomId, statPacket);

        //---------------- 후처리 --------------
    
        obj.gameRoom.PushAfter((int)_skill.duration * 1000,  () => {
            if (obj == null|| obj.gameRoom == null)
                return;

            obj.stat.Attack -= (int)_skill.attackbuff;            ////Todo: 나중에 디버프 같은거 생각해서 고치기 (이게 맞나)
            obj.stat.Defence -= (int)_skill.attackbuff;
            S_StatChange StatAfter = new S_StatChange();
            StatAfter.ObjectId = obj.Id;
            StatAfter.StatInfo = obj.stat;
            obj.gameRoom.Push(obj.gameRoom.BroadCast, obj.CurrentRoomId, StatAfter);
            return; 
        });

       //------------ 
       Console.WriteLine("Skill100____________");
    }
    internal static void Skill102(GameObject obj) // 오로라,지속 광역버프
    {
       Skill _skill;
        if (DataManager.SkillDict.TryGetValue(102, out _skill) == false)
            return;

        //if(obj.stat.Mp <= 0 || obj.stat.Mp < _skill.)
        #region 검사구역

        if (obj.ObjectType != GameObjectType.Player) //플레이어라면
            return;

        //----------------- 쿨타임 ---------------
        Player p = (Player)obj;
        bool t = p.ApplySkill(102, _skill.cooldown);
        Console.WriteLine(t ? $"{obj.info.Name}성공" : $"{obj.info.Name}실패");
        if (t == false) //실패하면
            return;
        //----------------- 코스트 ---------------





        #endregion
        //------------ 통과 --------------
        Room room = p.gameRoom.Map.GetRoom(p.CurrentRoomId);
        Map TargetMap =  p.gameRoom.Map;
        HashSet<GameObject> _tempTargets= TargetMap.GetPlanetObjects(p.CurrentRoomId);
        _tempTargets.UnionWith(TargetMap.GetPlanetPlayers(p.CurrentRoomId));

        HashSet<GameObject> targets = _tempTargets.Where(go => go.OwnerId == p.Id).ToHashSet<GameObject>();
        targets.Add(p);

        if (targets.Count == 0)
            return;

        int[] Buff = new int[targets.Count];
        int arrayCount = 0;
        foreach (GameObject target in targets)
        {
            Buff[arrayCount] =  (int)MathF.Round((float)Math.Min(target.stat.Attack * 0.1, 1));
            target.stat.Attack += Buff[arrayCount];
            arrayCount++;
        }

        S_StatChange statPacket = new S_StatChange();
        statPacket.ObjectId = obj.Id;
        statPacket.StatInfo = obj.stat;
        obj.gameRoom.Push(obj.gameRoom.BroadCast, obj.CurrentRoomId, statPacket);


        //---------------- 후처리 --------------
        obj.gameRoom.PushAfter((int)_skill.duration * 1000, () => {
            if (obj == null || obj.gameRoom == null)
                return;

            arrayCount = 0;
            foreach (GameObject target in targets)
            {
                target.stat.Attack -= Buff[arrayCount];

                S_StatChange StatAfter = new S_StatChange();
                StatAfter.ObjectId = obj.Id;
                StatAfter.StatInfo = obj.stat;
                obj.gameRoom.Push(obj.gameRoom.BroadCast, obj.CurrentRoomId, StatAfter);
                arrayCount++;
            }
            
            return;
        });

        //------------ 
        Console.WriteLine("Skill____________");
        
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
        //스킬 찾기
        Skill skill = null;
        if (DataManager.SkillDict.TryGetValue(200, out skill) == false)  //Todo
            return;

        //검사
        if (obj.ObjectType != GameObjectType.Player)
            return;

        Player p = (Player)obj;
        bool t = p.ApplySkill(200, skill.cooldown);
        Console.WriteLine(t ? $"{obj.info.Name} 성공" : $"{obj.info.Name} 실패");
        if (t == false) //실패하면
            return;

        Vector2 dir = Vector2.Normalize(new Vector2(p.SkillDir.X, p.SkillDir.Y));
        //------------ 통과 --------------

        S_Skill skillPacket = new S_Skill() { Info = new SkillInfo() };
        skillPacket.ObjectId = obj.Id;
        skillPacket.Info.SkillId = 200;

        p.gameRoom.Push(p.gameRoom.BroadCast, p.CurrentRoomId, skillPacket);


        Arrow arrow = ObjectManager.Instance.Add<Arrow>();
        if (arrow == null)
            return;
        arrow.CurrentRoomId = p.CurrentRoomId;
        arrow.info.Name = "Arrow";
        arrow.OwnerId = p.Id;
        arrow.Data = skill;
        arrow.PosInfo.State = CreatureState.Moving;
        arrow.PosInfo.DirX = dir.X;
        arrow.PosInfo.DirY = dir.Y;
        arrow.PosInfo.PosX = p.PosInfo.PosX;
        arrow.PosInfo.PosY = p.PosInfo.PosY;
        arrow.Speed = skill.projectile.speed;
        arrow.info.SkillId = 200;
        arrow.Attack = skill.damage;

        //Console.WriteLine($"P pos : {p.PosInfo.PosX},{p.PosInfo.PosY}");
        //Console.WriteLine($"arrow pos : {arrow.PosInfo.PosX},{arrow.PosInfo.PosY}");
        p.gameRoom.Push(p.gameRoom.EnterGame, arrow, false);


        // Console.WriteLine("Skill11____________");
        //Console.WriteLine ($"{arrow.PosInfo.DirX},{arrow.PosInfo.DirY},{arrow.PosInfo.PosX},{arrow.PosInfo.PosY}");

    }
    internal static void Skill301(GameObject obj)
    {
        //시체를 일으켜 망자로 만든다   몬스터 Id : 100
        Skill skill = null;
        if (DataManager.SkillDict.TryGetValue(301, out skill) == false)  //Todo
            return;

        //검사
        if (obj.ObjectType != GameObjectType.Player)
            return;

        Player p = (Player)obj;
        bool t = p.ApplySkill(301, skill.cooldown);
        Console.WriteLine(t ? $"{obj.info.Name} 성공" : $"{obj.info.Name} 실패");
        if (t == false) //실패하면
            return;

        Vector2 dir = Vector2.Normalize(new Vector2(p.SkillDir.X, p.SkillDir.Y));
        //------------ 통과 --------------

        S_Skill skillPacket = new S_Skill() { Info = new SkillInfo() };
        skillPacket.ObjectId = obj.Id;
        skillPacket.Info.SkillId = 301;

        p.gameRoom.Push(p.gameRoom.BroadCast, p.CurrentRoomId, skillPacket);


        Monster summons = ObjectManager.Instance.Add<Monster>();
        if (summons == null)
            return;
        summons.OwnerId = p.Id;
        summons.CurrentRoomId = p.CurrentRoomId;
        summons.SpawnType = MonsterSpawnType.player;

        summons.info.Name = skill.name;
        summons.Init(100);

        summons.PosInfo.MergeFrom(new PositionInfo()
        {
            PosX = p.CellPos.X,
            PosY = p.CellPos.Y,
            CurrentRoomId = p.CurrentRoomId,
            State = CreatureState.Idle
        });


        p.gameRoom.Push(p.gameRoom.EnterGame, summons, false);



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

