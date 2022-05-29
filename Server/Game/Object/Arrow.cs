using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Server.Game
{
    public class Arrow : Projectile
    {
        
        public bool active = false;
        private bool Destoryed = false;

        public Arrow()
        {
            Console.WriteLine("Arrow");
        }

        public override void Update()
        {
            if (Data == null || Data.projectile == null || OwnerId == -1 || gameRoom == null || Destoryed == true)
                return;

            gameRoom.PushAfter(Program.ServerTick, Update);
            if (active == false)
            {
                Console.WriteLine("시간" + System.Environment.TickCount64);
                gameRoom.PushAfter(5 * 1000, Destroy); ;
                active = true;
                return;
            }

            CellPos += Speed * Program.ServerTick / 1000 * Dir;

            //Console.WriteLine("Arrow" + CellPos + $"time {System.Environment.TickCount64}");


            S_Move movePacket = new S_Move()
            {
                ObjectId = Id,
                PositionInfo = PosInfo,
            };



            gameRoom.BroadCast(CurrentRoomId, movePacket);

        }


        public void Destroy()
        {
            if (Data == null || Data.projectile == null || OwnerId == -1 || gameRoom == null)
                return;

            gameRoom.Push(() => { ObjectManager.Instance.Remove(Id); });
            gameRoom.Push(gameRoom.LeaveGame, Id);
            Destoryed = true;
            Console.WriteLine("Destory");

        }

        public override GameObject GetOwner()
        {
            if(OwnerId == -1 || OwnerId == 0)
                return null;

            GameObject Owner;
            Owner = gameRoom.Map.FindObjById(CurrentRoomId,OwnerId);   //데이터 레이스?
            return Owner;
        }


        /*if (Room.Map.ApplyMove(this,destPos,cheakObjects : true,collision : false))
           {
               S_Move movePacket = new S_Move();
               movePacket.ObjectId = Id;
               movePacket.PositionInfo = PosInfo;

               Room.BroadCast(CellPos, movePacket);
           }
           else
           {
               GameObject target = Room.Map.Find(destPos);
               if(target != null)
               {
                   target.OnDamaged(this,Data.damage + Owner.TotalAttack);
               }

               //소멸
               Room.Push(Room.LeaveGame,Id); 

           }*/
    }



}
