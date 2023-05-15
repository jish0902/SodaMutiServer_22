using System;
using System.Linq;
using System.Numerics;
using Google.Protobuf.Protocol;
using Server.Data;
using Server.Game;

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
        var p = (Player)obj;
        var t = p.ApplySkill(100, _skill.cooldown);
        Console.WriteLine(t ? $"{obj.info.Name}성공" : $"{obj.info.Name}실패");
        if (t == false) //실패하면
            return;


        //----------------- 코스트 ---------------

        //Todo : 거리 채ㅔ크

        #endregion

        //------------ 통과 --------------

        GameObject _target;
        if (p.Targets == null || p.Targets.Count == 0) //없으면
        {
            _target = p.gameRoom.FindCloestMonsterAndPlayer(p);
            Console.WriteLine("Done");
        }
        else
        {
            _target = p.Targets.First();
        }

        //if (_target != null || _target.gameRoom != null || p.gameRoom != null || _target.gameRoom == p.gameRoom || _target.State != CreatureState.Dead)
        //    _target.OnDamaged(p, _skill.damage + p.Attack);
        if (_target != null && (_target.gameRoom != null || p.gameRoom != null || _target.gameRoom == p.gameRoom ||
                                _target.State != CreatureState.Dead))
            _target.OnDamaged(p, _skill.damage + p.Attack);
        else
            //실패시
            return;


        Console.WriteLine($"{p.info.Name}이 {_target.info.Name}에게 {_skill.damage + p.Attack}데미지 줌  남은 피: {_target.Hp}");


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
        var p = (Player)obj;
        var t = p.ApplySkill(101, _skill.cooldown);
        Console.WriteLine(t ? $"{obj.info.Name}성공" : $"{obj.info.Name}실패");
        if (t == false) //실패하면
            return;


        //----------------- 코스트 ---------------

        #endregion

        //------------ 통과 --------------
        obj.stat.Attack += (int)_skill.attackbuff;
        obj.stat.Defence += (int)_skill.attackbuff;


        var skillPacket = new S_Skill { Info = new SkillInfo() };
        skillPacket.ObjectId = obj.Id;
        skillPacket.Info.SkillId = 101;
        obj.gameRoom.Push(obj.gameRoom.BroadCast, obj.CurrentRoomId, skillPacket);


        var statPacket = new S_StatChange();
        statPacket.ObjectId = obj.Id;
        statPacket.StatInfo = obj.stat;
        obj.gameRoom.Push(obj.gameRoom.BroadCast, obj.CurrentRoomId, statPacket);

        //---------------- 후처리 --------------

        obj.gameRoom.PushAfter((int)_skill.duration * 1000, () =>
        {
            if (obj == null || obj.gameRoom == null)
                return;

            obj.stat.Attack -= (int)_skill.attackbuff; ////Todo: 나중에 디버프 같은거 생각해서 고치기 (이게 맞나)
            obj.stat.Defence -= (int)_skill.attackbuff;
            var StatAfter = new S_StatChange();
            StatAfter.ObjectId = obj.Id;
            StatAfter.StatInfo = obj.stat;
            obj.gameRoom.Push(obj.gameRoom.BroadCast, obj.CurrentRoomId, StatAfter);
        });

        //------------ 
        Console.WriteLine("Skill101");
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
        var p = (Player)obj;
        var t = p.ApplySkill(102, _skill.cooldown);
        Console.WriteLine(t ? $"{obj.info.Name}성공" : $"{obj.info.Name}실패");
        if (t == false) //실패하면
            return;
        //----------------- 코스트 ---------------

        #endregion

        //------------ 통과 --------------
        var room = p.gameRoom.Map.GetRoom(p.CurrentRoomId);
        var TargetMap = p.gameRoom.Map;
        var _tempTargets = TargetMap.GetPlanetObjects(p.CurrentRoomId);
        _tempTargets.UnionWith(TargetMap.GetPlanetPlayers(p.CurrentRoomId));

        var targets = _tempTargets.Where(go => go.OwnerId == p.Id).ToHashSet();
        targets.Add(p);

        if (targets.Count == 0)
            return;

        var Buff = new int[targets.Count];
        var arrayCount = 0;
        foreach (var target in targets)
        {
            Buff[arrayCount] = (int)MathF.Round((float)Math.Min(target.stat.Attack * 0.1, 1));
            target.stat.Attack += Buff[arrayCount];
            arrayCount++;

            var statPacket = new S_StatChange();
            statPacket.ObjectId = target.Id;
            statPacket.StatInfo = target.stat;
            p.gameRoom.Push(target.gameRoom.BroadCast, target.CurrentRoomId, statPacket);
        }


        //---------------- 후처리 --------------
        p.gameRoom.PushAfter((int)_skill.duration * 1000, () =>
        {
            if (p == null || p.gameRoom == null)
                return;

            arrayCount = 0;
            foreach (var target in targets)
            {
                target.stat.Attack -= Buff[arrayCount];

                var StatAfter = new S_StatChange();
                StatAfter.ObjectId = target.Id;
                StatAfter.StatInfo = target.stat;
                p.gameRoom.Push(target.gameRoom.BroadCast, target.CurrentRoomId, StatAfter);
                arrayCount++;
            }
        });

        //------------ 
        Console.WriteLine("Skill102");
    }

    internal static void Skill103(GameObject obj) //중갑전차, 반사데미지
    {
        Skill _skill;
        if (DataManager.SkillDict.TryGetValue(103, out _skill) == false)
            return;

        //if(obj.stat.Mp <= 0 || obj.stat.Mp < _skill.)

        #region 검사구역

        if (obj.ObjectType != GameObjectType.Player) //플레이어라면
            return;

        //----------------- 쿨타임 ---------------
        var p = (Player)obj;
        var t = p.ApplySkill(103, _skill.cooldown);
        Console.WriteLine(t ? $"{obj.info.Name} 성공" : $"{obj.info.Name} 실패");
        if (t == false) //실패하면
            return;


        //----------------- 코스트 ---------------

        #endregion

        //------------ 통과 --------------

        p.DamageReflexAction = att =>
        {
            var attacker = att as CreatureObj;
            if (attacker != null && attacker.DamageReflexAction == null)
                attacker.OnDamaged(p, _skill.damage * 5);
            Console.WriteLine($"{_skill.damage * 5} 만큼 반사");
        };

        var skillPacket = new S_Skill { Info = new SkillInfo() };
        skillPacket.ObjectId = p.Id;
        skillPacket.Info.SkillId = 103;
        p.gameRoom.Push(p.gameRoom.BroadCast, p.CurrentRoomId, skillPacket);

        //---------------- 후처리 --------------

        //(int)_skill.duration
        p.gameRoom.PushAfter(5 * 1000, () =>
        {
            if (p == null || p.gameRoom == null)
                return;

            Console.WriteLine("103 끝");
            p.DamageReflexAction = null;
        });


        //------------ 
        Console.WriteLine("Skill103");
    }


    //신격 : 체력 비례 데미
    internal static void Skill104(GameObject obj)
    {
        Skill _skill;
        if (DataManager.SkillDict.TryGetValue(104, out _skill) == false)
            return;

        //if(obj.stat.Mp <= 0 || obj.stat.Mp < _skill.)

        #region 검사구역

        if (obj.ObjectType != GameObjectType.Player) //플레이어라면
            return;

        //----------------- 쿨타임 ---------------
        var p = (Player)obj;
        var t = p.ApplySkill(104, _skill.cooldown);
        Console.WriteLine(t ? $"{obj.info.Name} 성공" : $"{obj.info.Name} 실패");
        if (t == false) //실패하면
            return;


        //----------------- 코스트 ---------------

        #endregion

        //------------ 통과 --------------


        var lossHpPer = 1.0 - (double)p.Hp / p.info.StatInfo.MaxHp;

        var additionalDamage = _skill.amount * (int)Math.Ceiling(lossHpPer * 10);
        //10프로 손해시 amount * 1(0.1 * 10)

        var TotalDamage = additionalDamage + _skill.damage;

        Console.WriteLine($"104 스킬 dam : {TotalDamage} , {additionalDamage} , {_skill.damage}");
        Console.WriteLine($"104 스킬 hp :{p.Hp} {p.info.StatInfo.MaxHp}");

        GameObject _target;
        if (p.Targets == null || p.Targets.Count == 0) //없으면
        {
            _target = p.gameRoom.FindCloestMonsterAndPlayer(p);
            Console.WriteLine("Done");
        }
        else
        {
            _target = p.Targets.First();
        }

        //if (_target != null || _target.gameRoom != null || p.gameRoom != null || _target.gameRoom == p.gameRoom || _target.State != CreatureState.Dead)
        //    _target.OnDamaged(p, _skill.damage + p.Attack);
        if (_target != null && (_target.gameRoom != null || p.gameRoom != null || _target.gameRoom == p.gameRoom ||
                                _target.State != CreatureState.Dead))
            _target.OnDamaged(p, _skill.damage + p.Attack);
        else
            //실패시
            return;


        var skillPacket = new S_Skill { Info = new SkillInfo() };
        skillPacket.ObjectId = p.Id;
        skillPacket.Info.SkillId = 104;
        p.gameRoom.Push(p.gameRoom.BroadCast, p.CurrentRoomId, skillPacket);

        //---------------- 후처리 --------------


        //------------ 
        Console.WriteLine("Skill104");
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
        //스킬 찾기
        Skill skill = null;
        if (DataManager.SkillDict.TryGetValue(200, out skill) == false) //Todo
            return;

        //검사
        if (obj.ObjectType != GameObjectType.Player)
            return;
        
        var p = (Player)obj;
        var t = p.ApplySkill(201, skill.cooldown);
        Console.WriteLine(t ? $"{obj.info.Name} 성공" : $"{obj.info.Name} 실패");
        if (t == false) //실패하면
            return;
    }

    internal static void Skill200(GameObject obj)
    {
        //스킬 찾기
        Skill skill = null;
        if (DataManager.SkillDict.TryGetValue(200, out skill) == false) //Todo
            return;

        //검사
        if (obj.ObjectType != GameObjectType.Player)
            return;

        var p = (Player)obj;
        var t = p.ApplySkill(200, skill.cooldown);
        Console.WriteLine(t ? $"{obj.info.Name} 성공" : $"{obj.info.Name} 실패");
        if (t == false) //실패하면
            return;

        var dir = Vector2.Normalize(new Vector2(p.SkillDir.X, p.SkillDir.Y));
        //------------ 통과 --------------

        var skillPacket = new S_Skill { Info = new SkillInfo() };
        skillPacket.ObjectId = obj.Id;
        skillPacket.Info.SkillId = 200;

        p.gameRoom.Push(p.gameRoom.BroadCast, p.CurrentRoomId, skillPacket);


        var arrow = ObjectManager.Instance.Add<Arrow>();
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
        if (DataManager.SkillDict.TryGetValue(301, out skill) == false) //Todo
            return;

        //검사
        if (obj.ObjectType != GameObjectType.Player)
            return;

        var p = (Player)obj;
        var t = p.ApplySkill(301, skill.cooldown);
        Console.WriteLine(t ? $"{obj.info.Name} 성공" : $"{obj.info.Name} 실패");
        if (t == false) //실패하면
            return;

        var dir = Vector2.Normalize(new Vector2(p.SkillDir.X, p.SkillDir.Y));
        //------------ 통과 --------------

        var skillPacket = new S_Skill { Info = new SkillInfo() };
        skillPacket.ObjectId = obj.Id;
        skillPacket.Info.SkillId = 301;

        p.gameRoom.Push(p.gameRoom.BroadCast, p.CurrentRoomId, skillPacket);


        var summons = ObjectManager.Instance.Add<Monster>();
        if (summons == null)
            return;
        summons.OwnerId = p.Id;
        summons.CurrentRoomId = p.CurrentRoomId;
        summons.SpawnType = MonsterSpawnType.player;

        summons.info.Name = skill.name;
        summons.Init(100);

        summons.PosInfo.MergeFrom(new PositionInfo
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
        Console.WriteLine("Skill303");
    }

    internal static void Skill304(GameObject obj)
    {
        Console.WriteLine("Skill304");
    }

    internal static void Skill302(GameObject obj)
    {
        Console.WriteLine("Skill302");
    }

    internal static void Skill300(GameObject obj)
    {
        Console.WriteLine("Skill300");
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