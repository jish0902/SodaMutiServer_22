using System;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using System.Text;
using Server.Data;
using System.Numerics;
using Server.Game.Room;

namespace Server.Game
{
    class Monster : GameObject
    {
        public int TemplateId { get; private set; }
        Vector2 targetPos = new Vector2(-9999,-9999);

        public Monster()
        {
            ObjectType = GameObjectType.Monster;
        }


        public void Init(int templateId) //무조건 currnetplanetId 먼저 할당해야함
        {
            TemplateId = templateId;

            MonsterData monsterData = null;
            DataManager.MonsterDict.TryGetValue(templateId, out monsterData);
            
            info.Name = monsterData.name;
            stat.MergeFrom(monsterData.stat);
            stat.Hp = monsterData.stat.MaxHp;
            State = CreatureState.Idle;

            Random r = new Random();
            Side = (r.Next(0, 4));

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
                _job = Room.PushAfter(200, Update);
            }


        }

        Vector2 test = new Vector2(-9999, -9999);
        protected virtual void UpdateIdle()
        {
            if((CurrentPlanetId) == 1)
                Console.WriteLine($"Idle = {CellPos}");
            //if ((CurrentPlanetId) == 1)
            //{

            //    targetPos = CellPos + new Vector2(1, 0);
            //    Vector2 a = Vector2.Lerp(CellPos, targetPos, 0.1f * Speed);
            //    CellPos = a;
            //    Console.WriteLine("UpdateMoving : " + CellPos + "targetPos" + targetPos + "t : " + a);

                
            //}
            //return;
            
            Player p = Room.FindCloestPlayer(this);

            if (p == null || p.Room != Room || p.Hp == 0)
            {
                p = null;
                return;
            }

            Game.Room.Room planet = Room.Map.Rooms.Find(p => p.Id == (CurrentPlanetId));
            //int minx = planet.PosX - planet.Round / 2 + 1;
            //int maxX = planet.PosX + planet.Round / 2 - 1;

            //float t = CellPos.X - p.CellPos.X;
            //if (t > 0 || t < maxX)
            //{
            //    targetPos = CellPos - new Vector2(1, 0);
            //}
            //else if (t <= 0 || t > minx)
            //{
            //    targetPos = CellPos + new Vector2(1, 0);
            //}

            State = CreatureState.Moving;
          


           

        }

        //long _nextMoveTick = 0;

        
        protected virtual void UpdateMoving()
        {

            //if (_nextMoveTick > Environment.TickCount64)
            //    return;
            //int moveTick = (int)(1000 / Speed);
            //_nextMoveTick = Environment.TickCount64 + moveTick;
            Player p = Room.FindCloestPlayer(this);

            if (p == null || p.Room != Room || p.Hp == 0 || (targetPos - CellPos).Length() <= 0.1)
            {
                State = CreatureState.Idle;
                return;
            }
            Console.WriteLine("UpdateMoving : " + CellPos + "targetPos" + targetPos);

            Vector2 t =  Vector2.Lerp(CellPos, targetPos, 0.1f * Speed);
            CellPos = t;
            S_Move movepacket = new S_Move()
            {
                ObjectId = Id,
                PositionInfo = PosInfo,
            };
            Room.BroadCast(CurrentPlanetId, movepacket);


            //BroadCastMove();
        }



        protected virtual void UpdatSkill()
        {


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
            Room.BroadCast(CurrentPlanetId, movepacket);
        }
    }



}
