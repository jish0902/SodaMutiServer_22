using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Server.Game
{
    public class GameObject
    {
        public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;
        public ObjectInfo info { get; set; } = new ObjectInfo();
        public int CurrentPlanetId
        {
            get { return info.PositionInfo.CurrentPlanetId; }
            set { info.PositionInfo.CurrentPlanetId = value; }
        }
        public int Side
        {
            get { return info.PositionInfo.Side; }
            set { info.PositionInfo.Side = value; }
        }

        public int Id
        {
            get { return info.ObjectId; }
            set { info.ObjectId = value; }
        }
        public GameRoom Room { get; set; }
        public PositionInfo PosInfo { get; private set; } = new PositionInfo();
        public StatInfo stat { get; private set; } = new StatInfo();

        //public virtual int TotalAttack { get { return stat.Attack; } }
        //public virtual int TotalDefence { get { return 0; } }

        
        public Vector2 Dir
        {
            get { return new Vector2(PosInfo.DirX ,PosInfo.DirY); }
            set 
            { 
                PosInfo.DirX = value.X ; 
                PosInfo.DirY = value.Y ; 
            }
        }

        public CreatureState State
        {
            get { return PosInfo.State; }
            set { PosInfo.State = value; }
        }

        public int Attack
        {
            get { return stat.Attack; }
            set { stat.Attack = value; }
        }


        public float Speed
        {
            get { return stat.Speed; }
            set { stat.Speed = value; }
        }
        public int Hp
        {
            get { return stat.Hp; }
            set { stat.Hp = Math.Clamp(value, 0, stat.MaxHp); }
        }
        public GameObject()
        {
            info.PositionInfo = PosInfo;
            info.StatInfo = stat;
        }

        public virtual void Update()
        {

        }


        public Vector2 CellPos
        {
            get
            {
                return new Vector2(PosInfo.PosX, PosInfo.PosY);
            }
            set
            {
                info.PositionInfo.PosX = value.X;
                info.PositionInfo.PosY = value.Y;
            }
        }

        public Vector2 GetFrontCellPos()
        {
            return GetFrontCellPos(new Vector2(Dir.X, Dir.Y));
        }

        public Vector2 GetFrontCellPos(Vector2 dir)
        {
            Vector2 cellPos = CellPos;

            //Todo : dir크기를 재서 이상있으면 지우기
            cellPos += dir;

            return cellPos;
        }
        public virtual void OnDamaged(GameObject attacker, int damage)
        {
            if (Room == null)
                return;


            damage = Math.Max((damage), 0);
            stat.Hp = Math.Max(stat.Hp - damage, 0);

            //Console.WriteLine($"damage : {damage}  stat.Hp : {stat.Hp}");

            S_ChangeHp ChangePacket = new S_ChangeHp();
            ChangePacket.ObjectId = Id;
            ChangePacket.Hp = stat.Hp;
            Room.BroadCast(CurrentPlanetId, ChangePacket);

            if (stat.Hp <= 0)
            {
                stat.Hp = 0;
                OnDead(attacker);
            }
        }
        public virtual void OnDead(GameObject attacker)
        {
            if (Room == null)
                return;


            S_Die diePacket = new S_Die();
            diePacket.ObjectId = Id;
            diePacket.AttackerId = attacker.Id;

            Room.BroadCast(CurrentPlanetId, diePacket);

            GameRoom room = Room;
            room.Push(room.LeaveGame, Id);

            room.Push(new Job(() => {
                stat.Hp = stat.MaxHp;
                PosInfo.State = CreatureState.Idle;
            }));

            room.Push(room.EnterGame, this, true);


        }
        public virtual GameObject GetOwner()
        {
            return this;
        }
       

    }// class

    public struct Vector2Int
    {
        public int x;
        public int y;

        public Vector2Int(int x, int y) { this.x = x; this.y = y; }

        public static Vector2Int up { get { return new Vector2Int(0, 1); } }
        public static Vector2Int down { get { return new Vector2Int(0, -1); } }
        public static Vector2Int left { get { return new Vector2Int(-1, 0); } }
        public static Vector2Int right { get { return new Vector2Int(1, 0); } }

        public static Vector2Int operator +(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.x + b.x, a.y + b.y);
        }
        public static Vector2Int operator -(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.x - b.x, a.y - b.y);
        }

        public float magnitude { get { return (float)MathF.Sqrt(sqrMagnitude); } }
        public int sqrMagnitude { get { return (x * x + y + y); } }

        public int cellDistFromZero { get { return Math.Abs(x) + Math.Abs(y); } }
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

}
