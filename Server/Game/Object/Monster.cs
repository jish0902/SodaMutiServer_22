using System;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using System.Text;
using Server.Data;
using System.Numerics;

namespace Server.Game
{

    public enum MonsterAttackType
    {
        Melee = 0,
        Range = 1,
        Explosion = 2,
    }



     public class Monster : GameObject
    {
        public int TemplateId { get; private set; }
        
        public MonsterAttackType AttackType { get; private set; }

        private GameObject _attackTarget;
        private GameObject _Owner;

 
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
                AttackType = MonsterAttackType.Melee; //Todo : 나중에 바꾸기(AttackSpeed)
                Speed = monsterData.stat.Speed;
                //Console.WriteLine(stat.AttackSpeed);
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
            if (gameRoom != null)
            {
                _job = gameRoom.PushAfter(Program.ServerTick, Update);
            }


        }


        void CheakSkill(GameObject target)
        {
            if(target == null || target.gameRoom != gameRoom || target.Hp == 0 || target.CurrentRoomId != CurrentRoomId)
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
            _attackTarget = null;

            if (OwnerId != -1) //주인이 있으면
            {
                Player p = gameRoom.FindCloestPlayer(this, except: new int[] { OwnerId });
                Monster m = gameRoom.FindCloestMonster(this, except: new int[] { OwnerId });



                if (p != null && m != null)
                    _attackTarget = Vector2.Distance(p.CellPos, CellPos) <= Vector2.Distance(m.CellPos, CellPos) ? _attackTarget : m;
                else if (p != null)
                    _attackTarget = p;
                else if(m != null)
                    _attackTarget = m;
                
                //플레이어가 가까우면 플레이어 아니면 몬스터
                
            }
            else  //주인없으면 가까운 플레이어 때림
            {
                _attackTarget = gameRoom.FindCloestPlayer(this);
            }

            if (_attackTarget == null || _attackTarget.gameRoom != gameRoom || _attackTarget.Hp == 0 || _attackTarget.CurrentRoomId != CurrentRoomId)
            {
                _attackTarget = null;
                return;
            }

            State = CreatureState.Moving;
            Console.WriteLine("Moving로 상태변화");

        }

        protected virtual void UpdateMoving()
        {

            Player p = gameRoom.FindCloestPlayer(this);


            if (p == null || p.gameRoom != gameRoom || p.Hp == 0 || p.CurrentRoomId != CurrentRoomId)
            {
                State = CreatureState.Idle;
                p = null;
                Console.WriteLine("idle로 상태변화");
                return;
            }

            float distance = (p.CellPos - CellPos).Length();

            
            if(distance < AttackRange - 0.2f)
            {
                _attackTarget = p;
                State = CreatureState.Skill;
                return;
            }


            Dir = Vector2.Normalize(p.CellPos - CellPos);
            CellPos += Speed * Program.ServerTick / 1000 * Dir;


            S_Move movepacket = new S_Move()
            {
                ObjectId = Id,
                PositionInfo = PosInfo,
            };
            gameRoom.BroadCast(CurrentRoomId, movepacket);


            //BroadCastMove();
        }

        public override void OnDead(GameObject attacker)
        {
            if (gameRoom == null)
                return;

            _attackTarget = null;
            
            if(OwnerId == -1)  //주인이 없으면
            {
                GameRoom room = gameRoom;
                room.Push(room.LeaveGame, Id);

                room.PushAfter(3000, new Job(() => {
                    stat.Hp = stat.MaxHp;
                    PosInfo.State = CreatureState.Idle;
                }));

                room.PushAfter(3000, room.EnterGame, this, true);
            }
            else                 //주인이 있으면
            {
                OwnerId = -1;
            }

            
            S_Die diePacket = new S_Die();
            diePacket.ObjectId = Id;
            diePacket.AttackerId = attacker.Id;
            gameRoom.BroadCast(CurrentRoomId, diePacket);


            State = CreatureState.Dead;
        }





        long _skillCool = 0;
        protected virtual void UpdatSkill()
        {
            if(_skillCool == 0)
            {
                if(_attackTarget != null)
                _attackTarget.OnDamaged(this, Attack);       //--------------공격----------------------

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


        void BroadCastMove()
        {
            //다른플레이어에게도 알려준다
            S_Move movepacket = new S_Move();
            movepacket.ObjectId = Id;
            movepacket.PositionInfo = PosInfo;
            gameRoom.BroadCast(CurrentRoomId, movepacket);
        }
    }



}
