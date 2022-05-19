using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Server.Game
{
    public class Arrow : Projectile
    {
        public GameObject Owner;
        public bool active = false;
        private bool Destoryed = false;

        public Arrow()
        {
            Console.WriteLine("Arrow");
        }

        public override void Update()
        {
            if (Data == null || Data.projectile == null || Owner == null || Room == null || Destoryed == true)
                return;

            Room.PushAfter(Program.ServerTick, Update);
            if (active == false)
            {
                Room.PushAfter(5 * 1000, Destroy); ;
                active = true;
                return;
            }

            CellPos += Speed * Program.ServerTick / 1000 * Dir;

            Console.WriteLine("Arrow" + CellPos + $"time {System.Environment.TickCount64}");


            S_Move movePacket = new S_Move()
            {
                ObjectId = Id,
                PositionInfo = PosInfo,
            };



            Room.BroadCast(CurrentRoomId, movePacket);


        }


        public void Destroy()
        {
            if (Data == null || Data.projectile == null || Owner == null || Room == null)
                return;

            Room.Push(() => { ObjectManager.Instance.Remove(Id); });
            Room.Push(Room.LeaveGame, Id);
            Destoryed = true;
            Console.WriteLine("Destory");

        }

        public override GameObject GetOwner()
        {
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
