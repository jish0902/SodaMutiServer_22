using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
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

            if(active == false)
            {
                Room.PushAfter(5 * 1000, Destroy); ;
                active = true;
            }


            int tick = (int)(1000 / Data.projectile.speed);
            Room.PushAfter(tick, Update);

            CellPos += Speed * tick / 1000 * Dir;



            //Vector2 t = new Vector2(PosInfo.PosX, PosInfo.PosY);
            //Vector2 y = new Vector2(PosInfo.DirX, PosInfo.DirY);
            //Room.Map.ApplyProjectile(t, y);

            S_Move movePacket = new S_Move();
            movePacket.ObjectId = Id;
            movePacket.PositionInfo = PosInfo;
            Console.WriteLine("-------" + PosInfo.PosX + "-------" + PosInfo.PosY);


            Room.BroadCast(CurrentPlanetId, movePacket);
            
           
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
