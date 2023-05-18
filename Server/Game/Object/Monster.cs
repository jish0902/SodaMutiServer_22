using System;
using System.Numerics;
using Google.Protobuf.Protocol;
using Server.Data;

namespace Server.Game;

public enum MonsterAttackType
{
    Melee = 0,
    Range = 1,
    Explosion = 2,
    Spawning = 3
}

public enum MonsterSpawnType //직접 소환 or 세력 느낌
{
    player = 0,
    skill = 1
}

public class Monster : CreatureObj
{
    private readonly int FindRange = 12;


    private IJob _job;


    private long _moveTime; //1초마다 idle로 가서 다시 가까운애 찾기


    private long _skillCool;

    private GameObject _target;

    public Monster()
    {
        ObjectType = GameObjectType.Monster;
    }

    public int TemplateId { get; private set; }

    public MonsterAttackType AttackType { get; private set; }

    public MonsterSpawnType SpawnType { get; set; } = MonsterSpawnType.skill;


    public void Init(int templateId) //무조건 currnetplanetId 먼저 할당해야함
    {
        TemplateId = templateId;

        MonsterData monsterData = null;
        if (DataManager.MonsterDict.TryGetValue(templateId, out monsterData))
        {
            info.Name = monsterData.name;
            stat.MergeFrom(monsterData.stat);
            stat.Hp = monsterData.stat.MaxHp;
            State = CreatureState.Idle;
            AttackType = MonsterAttackType.Melee; //Todo : 나중에 바꾸기(AttackSpeed)
            //if(monsterData.stat.AttackRange < 1)
            //{
            //     AttackType = MonsterAttackType.Melee;
            //}
            //else if(monsterData.stat.AttackRange > 2)
            //{
            //     AttackType = MonsterAttackType.Melee;

            //}


            Speed = monsterData.stat.Speed;
            //Console.WriteLine(stat.AttackSpeed);
        }
        else
        {
            Console.WriteLine("몬스터 오류");
        }
    }

    public override void Update()
    {
        switch (State)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Moving:
                UpdateMoving();
                break;
            case CreatureState.Skill:
                //TODO: UpdatSkill(); 
                break;
            case CreatureState.Dead:
                UpdateDead();
                break;
        }

        if (gameRoom != null) _job = gameRoom.PushAfter(Program.ServerIntervalTick, Update);
    }


    private void CheakSkill(GameObject target)
    {
        if ((target != null && target.gameRoom != gameRoom) || target.Hp == 0 ||
            target.CurrentRoomId != CurrentRoomId) return;
        var distance = (target.CellPos - CellPos).Length();

        if (AttackType == MonsterAttackType.Melee)
        {
            if (distance < 1)
            {
            }
        }
        else if (AttackType == MonsterAttackType.Range)
        {
        }
        else if (AttackType == MonsterAttackType.Explosion)
        {
        }
    }

    protected virtual void UpdateIdle() // 타겟에 가장 가까운 거리까지 이동
    {
        _target = null;

        if (OwnerId != -1) //주인이 있으면
        {
            var owner = gameRoom.Map.FindObjById(CurrentRoomId, OwnerId);
            if (owner != null && SpawnType == MonsterSpawnType.player) //주인이 있고 주인을 따라가는 상태이고 같은 방에 있으면
                if (Vector2.Distance(owner.CellPos, CellPos) > FindRange + 3)
                    _target = owner;

            if (_target == null) //주인 방에 없거나, 주인을 따라가는 상태가아니거나, 주인과 충분히 가까우면
            {
                var p = gameRoom.FindCloestPlayer(this, new[] { OwnerId });
                var m = gameRoom.FindCloestMonster(this, new[] { OwnerId });

                if (m != null && p != null) //둘다 있으면
                {
                    var disM = Vector2.Distance(m.CellPos, CellPos);
                    var disP = Vector2.Distance(p.CellPos, CellPos);

                    if (MathF.Min(disM, disP) > FindRange) //가장 가까운 오브젝트가 FindRange 보다 멀면
                    {
                        _target = null;
                        return;
                    }

                    if (disM >= disP)
                        _target = p;
                    else
                        _target = m;
                }
                else if (m != null) //몬스터만 있으면
                {
                    _target = m;
                }
                else if (p != null) //플레이어만 잇으면
                {
                    _target = p;
                }

                //가장 가까운 사람 먼저 따라감
            }
        }
        else //주인없으면 (플레이어 or 주인있는 몬스터중) 가장 가까운애 때림
        {
            var p = gameRoom.FindCloestPlayer(this);
            var m = gameRoom.FindCloestMonster(this, new[] { OwnerId });

            if (m != null && p != null) //둘다 있으면
            {
                var disM = Vector2.Distance(m.CellPos, CellPos);
                var disP = Vector2.Distance(p.CellPos, CellPos);

                if (MathF.Min(disM, disP) > FindRange) //가장 가까운 오브젝트가 FindRange 보다 멀면
                {
                    _target = null;
                    return;
                }

                if (disM >= disP)
                    _target = p;
                else
                    _target = m;
            }
            else if (m != null) //몬스터만 있으면
            {
                _target = m;
            }
            else if (p != null) //플레이어만 잇으면
            {
                _target = p;
            }
        }

        if (_target == null || _target.gameRoom != gameRoom || _target.Hp == 0 ||
            _target.CurrentRoomId != CurrentRoomId)
        {
            _target = null;
            return;
        }


        //Todo : _target의 a* 알고리즘 정렬후 한칸 움직임

        _moveTime = 0;
        State = CreatureState.Moving;
        //Console.WriteLine("Moving로 상태변화");
    }

    protected virtual void UpdateMoving()
    {
        if (_target == null || _target.gameRoom != gameRoom || _target.Hp == 0 ||
            _target.CurrentRoomId != CurrentRoomId)
        {
            State = CreatureState.Idle;
            _target = null;
            Console.WriteLine("player null or dead idle로 상태변화");
            return;
        }

        if (_target.Id != OwnerId && Vector2.Distance(_target.CellPos, CellPos) > FindRange) //_target이 주인이 아니고 거리가 멀어지면
        {
            State = CreatureState.Idle;
            _target = null;
            Console.WriteLine("(FindRange)idle로 상태변화");
            return;
        }


        #region MoveTime

        if (_moveTime == 0)
        {
            _moveTime = Environment.TickCount64 + 3000;
        }
        else
        {
            if (_moveTime <= Environment.TickCount64)
            {
                _moveTime = 0;
                State = CreatureState.Idle;
                _target = null;
                Console.WriteLine("(_time)idle로 상태변화");
                return;
            }
        }

        #endregion

        var _path = gameRoom.Map.FindPath(new Vector2Int((int)MathF.Round(CellPos.X), (int)MathF.Round(CellPos.Y)),
            new Vector2Int((int)MathF.Round(_target.CellPos.X), (int)MathF.Round(_target.CellPos.Y)),
            true);

        Console.WriteLine("길찾기");

        if (OwnerId != -1 && (_target.Id == OwnerId || _target.OwnerId == OwnerId)) //주인이 있고 타겟이 주인이거나 주인의 하수인이면
        {
            var _dis = Vector2.Distance(_target.CellPos, CellPos);
            var tempTarget = _path == null ? CellPos : (Vector2)_path[1];
            Dir = Vector2.Normalize(tempTarget - CellPos);

            if (_dis > FindRange + 5) //너무 멀면 빠르게 이동               Todo : 순간이동 구현
                CellPos += Speed * 2 * Program.ServerIntervalTick / 1000 * Dir;
            else //적당히 멀면
                CellPos += Speed * Program.ServerIntervalTick / 1000 * Dir;
        }
        else //적이면
        {
            #region 스킬 사용

            var distance = (_target.CellPos - CellPos).Length();
            if (distance <= AttackRange)
            {
                State = CreatureState.Skill;
                return;
            }

            #endregion

            var tempTarget = _path == null ? CellPos : (Vector2)_path[1];
            Dir = Vector2.Normalize(tempTarget - CellPos);
            CellPos += Speed * Program.ServerIntervalTick / 1000 * Dir;
        }


        var movepacket = new S_Move
        {
            ObjectId = Id,
            PositionInfo = PosInfo
        };
        gameRoom.BroadCast(CurrentRoomId, movepacket);

        //BroadCastMove();
    }

    public override void OnDead(GameObject attacker)
    {
        if (gameRoom == null)
            return;

        _target = null;

        if (OwnerId == -1) //주인이 없으면
        {
            var room = gameRoom;
            room.Push(room.LeaveGame, Id);

            room.PushAfter(3000, new Job(() =>
            {
                stat.Hp = stat.MaxHp;
                PosInfo.State = CreatureState.Idle;
            }));

            room.PushAfter(3001, room.EnterGame, this, true);
        }
        else //주인이 있으면
        {
            OwnerId = -1;

            var room = gameRoom;
            room.Push(room.LeaveGame, Id);
        }

        attacker.AddExp(stat.Exp);

        var diePacket = new S_Die();
        diePacket.ObjectId = Id;
        diePacket.AttackerId = attacker.Id;
        gameRoom.BroadCast(CurrentRoomId, diePacket);


        State = CreatureState.Dead;
    }

    protected virtual void UpdatSkill()
    {
        if (_target == null || _target.gameRoom != gameRoom || _target.Hp == 0 ||
            _target.CurrentRoomId != CurrentRoomId)
        {
            State = CreatureState.Idle;
            _target = null;
            Console.WriteLine("idle로 상태변화");
            return;
        }

        if (OwnerId != -1 && (_target.Id == OwnerId || _target.OwnerId == OwnerId)) //주인이 없거나 주인이거나 주인의 하수인이면
        {
            State = CreatureState.Idle;
            _target = null;
            Console.WriteLine("idle로 상태변화");
            return;
        }

        var distance = (_target.CellPos - CellPos).Length();
        if (distance > AttackRange)
        {
            State = CreatureState.Idle;
            _target = null;
            Console.WriteLine("idle로 상태변화");
            return;
        }

        if (_skillCool == 0)
        {
            if (_target != null)
                _target.OnDamaged(this, Attack); //--------------공격----------------------

            _skillCool = Environment.TickCount64 + (int)(stat.AttackSpeed * 1000);
        }

        if (_skillCool > Environment.TickCount64)
            return;

        _skillCool = 0;

        State = CreatureState.Idle;
    }


    protected virtual void UpdateDead()
    {
    }
}