using System;
using System.Numerics;
using Collision.Shapes;
using Google.Protobuf.Protocol;
using Server.Data;

namespace Server.Game;

public class GameObject
{
    public Shape currentShape { get; set; } = null; // 충돌때만 하면 
    
    public GameObject()
    {
        info.PositionInfo = PosInfo;
        info.StatInfo = stat;
    }

    public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;
    public ObjectInfo info { get; set; } = new() { OwnerId = -1 };

    public int CurrentRoomId
    {
        get => info.PositionInfo.CurrentRoomId;
        set => info.PositionInfo.CurrentRoomId = value;
    }

    public int Id
    {
        get => info.ObjectId;
        set => info.ObjectId = value;
    }

    public int OwnerId //중요 : 초기값 -1
    {
        get => info.OwnerId;
        set => info.OwnerId = value;
    }


    public GameRoom gameRoom { get; set; }
    public PositionInfo PosInfo { get; } = new();
    public StatInfo stat { get; private set; } = new();

    //public virtual int TotalAttack { get { return stat.Attack; } }
    //public virtual int TotalDefence { get { return 0; } }


    public int Exp => stat.Exp;


    public Vector2 Dir
    {
        get => new(PosInfo.DirX, PosInfo.DirY);
        set
        {
            PosInfo.DirX = value.X;
            PosInfo.DirY = value.Y;
        }
    }

    public CreatureState State
    {
        get => PosInfo.State;
        set => PosInfo.State = value;
    }

    public int Attack
    {
        get => stat.Attack;
        set => stat.Attack = value;
    }

    public float AttackRange
    {
        get => stat.AttackRange;
        set => stat.AttackRange = value;
    }

    public float Speed
    {
        get => stat.Speed;
        set => stat.Speed = value;
    }

    public int Hp
    {
        get => stat.Hp;
        set => stat.Hp = Math.Clamp(value, 0, stat.MaxHp);
    }


    public Vector2 CellPos
    {
        get => new(PosInfo.PosX, PosInfo.PosY);
        set
        {
            info.PositionInfo.PosX = value.X;
            info.PositionInfo.PosY = value.Y;
        }
    }


    public void AddExp(int addnum)
    {
        if (addnum > 0)
            stat.Exp += addnum;
        else
            return;

        Console.WriteLine($"---------{info.Name} Exp {addnum} 증가------------");
        if (stat.Exp >= stat.MaxExp) //일단 레벨업
        {
            var addtional = stat.Exp - stat.MaxExp;

            SetLevelUp();

            //천천히 오르는 이팩트

            if (addtional >= stat.MaxExp) // 한번더 레벨업 할경우
                AddExp(addtional);
        }
    }

    public void SetLevelUp()
    {
        Console.WriteLine($"{info.Name}Level Up");
        stat.Level += 1;
        StatInfo nextStat;
        if (DataManager.StatDict.TryGetValue(100 * stat.Class + stat.Level, out nextStat))
        {
            stat = nextStat;
            stat.Exp = 0;

            var statPacket = new S_StatChange();
            statPacket.ObjectId = Id;
            statPacket.StatInfo = stat;
            gameRoom.Push(gameRoom.BroadCast, CurrentRoomId, statPacket);
        }
        else
        {
            Console.WriteLine("레벨업 오류");
        }
    }

    public virtual void Update()
    {
    }

    public Vector2 GetFrontCellPos()
    {
        return GetFrontCellPos(new Vector2(Dir.X, Dir.Y));
    }

    public Vector2 GetFrontCellPos(Vector2 dir)
    {
        var cellPos = CellPos;

        //Todo : dir크기를 재서 이상있으면 지우기
        cellPos += dir;

        return cellPos;
    }

    public virtual void OnDamaged(GameObject attacker, int damage)
    {
        if (gameRoom == null)
            return;


        damage = Math.Max(damage, 0);
        stat.Hp = Math.Max(stat.Hp - damage, 0);

        Console.WriteLine($" attacker :{attacker.Id} Damage : {damage}  stat.Hp : {stat.Hp}");

        var ChangePacket = new S_ChangeHp();
        ChangePacket.ObjectId = Id;
        ChangePacket.Hp = stat.Hp;
        gameRoom.BroadCast(CurrentRoomId, ChangePacket);

        if (stat.Hp <= 0)
        {
            stat.Hp = 0;
            OnDead(attacker);
        }
    }

    public virtual void OnCollision(GameObject other)
    {
        //상속받아 알아서 행동 ex) 스킬이면 데미지 사람이라면 충동처리
        Console.WriteLine(other.info.Name + " 과 충동함");
    }
    
    public virtual void OnDead(GameObject attacker)
    {
        if (gameRoom == null)
            return;


        var diePacket = new S_Die();
        diePacket.ObjectId = Id;
        diePacket.AttackerId = attacker.Id;

        gameRoom.BroadCast(CurrentRoomId, diePacket);

        var room = gameRoom;
        room.Push(room.LeaveGame, Id);

        room.Push(new Job(() =>
        {
            stat.Hp = stat.MaxHp;
            PosInfo.State = CreatureState.Idle;
        }));

        room.PushAfter(10, room.EnterGame, this, true);
    }

    public virtual GameObject GetOwner()
    {
        return this;
    }
} // class

public struct Vector2Int
{
    public int x;
    public int y;

    public Vector2Int(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static Vector2Int up => new(0, 1);
    public static Vector2Int down => new(0, -1);
    public static Vector2Int left => new(-1, 0);
    public static Vector2Int right => new(1, 0);

    public static Vector2Int operator +(Vector2Int a, Vector2Int b)
    {
        return new Vector2Int(a.x + b.x, a.y + b.y);
    }

    public static Vector2Int operator -(Vector2Int a, Vector2Int b)
    {
        return new Vector2Int(a.x - b.x, a.y - b.y);
    }

    public static Vector2Int operator -(Vector2 a, Vector2Int b)
    {
        return new Vector2Int((int)a.X - b.x, (int)a.Y - b.y);
    }

    public float sqrMagnitude => MathF.Sqrt(sqrMagnitude);
    public int Magnitude => x * x + y * y;

    public int cellDistFromZero => Math.Abs(x) + Math.Abs(y);

    public static explicit operator Vector2(Vector2Int v)
    {
        return new Vector2(v.x, v.y);
    }
}

//public struct Vector2
//{
//    public float x;
//    public float y;

//    public Vector2(float x, float y) { this.x = x; this.y = y; }

//    public static Vector2 up { get { return new Vector2(0, 1); } }
//    public static Vector2 down { get { return new Vector2(0, -1); } }
//    public static Vector2 left { get { return new Vector2(-1, 0); } }
//    public static Vector2 right { get { return new Vector2(1, 0); } }

//    public static Vector2 operator +(Vector2 a, Vector2 b)
//    {
//        return new Vector2(a.x + b.x, a.y + b.y);
//    }
//    public static Vector2 operator -(Vector2 a, Vector2 b)
//    {
//        return new Vector2(a.x - b.x, a.y - b.y);
//    }

//    public float magnitude { get { return (float)MathF.Sqrt(sqrMagnitude); } }
//    public float sqrMagnitude { get { return (x * x + y * y); } }

//    //public int cellDistFromZero { get { return Math.Abs(x) + Math.Abs(y); } }
//}