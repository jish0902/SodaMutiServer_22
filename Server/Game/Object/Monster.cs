using System;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using System.Text;
using Server.Data;
using System.Numerics;
using Server.Game.Room;

namespace Server.Game
{

    public enum MonsterAttackType
    {
        Melee = 0,
        Range = 1,
        Explosion = 2,
    }



    class Monster : GameObject
    {
        public int TemplateId { get; private set; }
        
        public MonsterAttackType AttackType { get; private set; }
        
        

 
        public Monster()
        {
            ObjectType = GameObjectType.Monster;
        }


        public void Init(int templateId) //무조건 currnetplanetId 먼저 할당해야함
        {
            TemplateId = templateId;

            MonsterData monsterData = null;
            if( DataManager.MonsterDict.TryGetValue(templateId, out monsterData) == true)
            {
                info.Name = monsterData.name;
                stat.MergeFrom(monsterData.stat);
                stat.Hp = monsterData.stat.MaxHp;
                State = CreatureState.Idle;
                AttackType = MonsterAttackType.Melee; //Todo : 나중에 바꾸기
                Console.WriteLine(stat.AttackSpeed);
            }
            else
            {
                Console.WriteLine("몬스터 오류");
            }
           
        }

        IJob _job;
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
                    UpdatSkill();
                    break;
                case CreatureState.Dead:
                    UpdateDead();
                    break;
                default:
                    break;
            }
            if (Room != null)
            {
                _job = Room.PushAfter(Program.ServerTick, Update);
            }


        }


        void CheakSkill(GameObject target)
        {
            if(target == null || target.Room != Room || target.Hp == 0 || target.CurrentRoomId != CurrentRoomId)
            {
                return;
            }
            float distance = (target.CellPos - CellPos).Length();

            if (AttackType == MonsterAttackType.Melee)
            {
                if(distance < 1)
                {

                }
            }
            else if(AttackType == MonsterAttackType.Range)
            {

            }
            else if(AttackType == MonsterAttackType.Explosion)
            {

            }




        }

        
        protected virtual void UpdateIdle()
        {
            
            Player p = Room.FindCloestPlayer(this);

            if (p == null || p.Room != Room || p.Hp == 0 || p.CurrentRoomId != CurrentRoomId)
            {
                p = null;
                return;
            }

            Game.Room.Room planet = Room.Map.Rooms.Find(r => r.Id == (CurrentRoomId));
           
            State = CreatureState.Moving;
            Console.WriteLine("Moving로 상태변화");

        }

        protected virtual void UpdateMoving()
        {
            Player p = Room.FindCloestPlayer(this);

            float distance = (p.CellPos - CellPos).Length();

            if (p == null || p.Room != Room || p.Hp == 0 || distance < 0.2 || p.CurrentRoomId != CurrentRoomId)
            {
                State = CreatureState.Idle;
                Console.WriteLine("idle로 상태변화");
                return;
            }
           
            //Console.WriteLine("거리 " + distance);

            Dir = Vector2.Normalize(p.CellPos - CellPos);
            CellPos += Speed * Program.ServerTick / 1000 * Dir;


            S_Move movepacket = new S_Move()
            {
                ObjectId = Id,
                PositionInfo = PosInfo,
            };
            Room.BroadCast(CurrentRoomId, movepacket);


            //BroadCastMove();
        }

        public override void OnDead(GameObject attacker)
        {
            if (Room == null)
                return;


            S_Die diePacket = new S_Die();
            diePacket.ObjectId = Id;
            diePacket.AttackerId = attacker.Id;

            Room.BroadCast(CurrentRoomId, diePacket);

            State = CreatureState.Dead;
            GameRoom room = Room;
            room.Push(room.LeaveGame, Id);

            room.PushAfter(3000,new Job(() => {
                stat.Hp = stat.MaxHp;
                PosInfo.State = CreatureState.Idle;
            }));

            room.PushAfter(3000,room.EnterGame, this, true);
        }





        long _skillCool = 0;
        protected virtual void UpdatSkill()
        {
            if(_skillCool == 0)
            {
                




                _skillCool = Environment.TickCount64 + (int)(stat.AttackSpeed * 1000);
            }

            if (_skillCool > Environment.TickCount64)
                return;

            _skillCool = 0;




        }
        
        
        protected virtual void UpdateDead()
        {

        }


        void BroadCastMove()
        {
            //다른플레이어에게도 알려준다
            S_Move movepacket = new S_Move();
            movepacket.ObjectId = Id;
            movepacket.PositionInfo = PosInfo;
            Room.BroadCast(CurrentRoomId, movepacket);
        }
    }



}
