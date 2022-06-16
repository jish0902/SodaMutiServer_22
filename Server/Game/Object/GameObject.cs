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
        public ObjectInfo info { get; set; } = new ObjectInfo() { OwnerId = -1};
        public int CurrentRoomId
        {
            get { return info.PositionInfo.CurrentRoomId; }
            set { info.PositionInfo.CurrentRoomId = value; }
        }
       
        public int Id
        {
            get { return info.ObjectId; }
            set { info.ObjectId = value; }
        }

        public int OwnerId  //중요 : 초기값 -1
        {
            get { return info.OwnerId; }
            set { info.OwnerId = value; }
        }
        


        public GameRoom gameRoom { get; set; }
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

        public float AttackRange
        {
            get { return stat.AttackRange; }
            set { stat.AttackRange = value; }
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
            if (gameRoom == null)
                return;


            damage = Math.Max((damage), 0);
            stat.Hp = Math.Max(stat.Hp - damage, 0);

            Console.WriteLine($" attacker :{attacker.Id} Damage : {damage}  stat.Hp : {stat.Hp}");

            S_ChangeHp ChangePacket = new S_ChangeHp();
            ChangePacket.ObjectId = Id;
            ChangePacket.Hp = stat.Hp;
            gameRoom.BroadCast(CurrentRoomId, ChangePacket);

            if (stat.Hp <= 0)
            {
                stat.Hp = 0;
                OnDead(attacker);
            }
        }
        public virtual void OnDead(GameObject attacker)
        {
            if (gameRoom == null)
                return;


            S_Die diePacket = new S_Die();
            diePacket.ObjectId = Id;
            diePacket.AttackerId = attacker.Id;

            gameRoom.BroadCast(CurrentRoomId, diePacket);

            GameRoom room = gameRoom;
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
        public static Vector2Int operator -(Vector2 a, Vector2Int b)
        {
            return new Vector2Int((int)a.X - b.x, (int)a.Y - b.y);
        }

        public float sqrMagnitude { get { return (float)MathF.Sqrt(sqrMagnitude); } }
        public int Magnitude { get { return (x * x + y * y); } }

        public int cellDistFromZero { get { return Math.Abs(x) + Math.Abs(y); } }

        public static explicit operator Vector2(Vector2Int v)
        {
            throw new NotImplementedException();
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

}
