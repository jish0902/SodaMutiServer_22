using System;
using Collision.Shapes;
using Google.Protobuf.Protocol;
using Microsoft.VisualBasic.CompilerServices;
using ServerCore;

namespace Server.Game;

public class Arrow : Projectile
{
    public Arrow(Shape  shape)
    {
        Console.WriteLine("Arrow");
        ObjectType = GameObjectType.Projectile;
        _shape = shape;
    }

    public override bool CheakData()
    {
        
        if (Data == null ||  OwnerId == -1 || gameRoom == null || destroyed || Data.projectile == null)
            return false;
        return true;
        
    }

    public override void Update()
    {
        base.Update();
        
        if (active == false) //처음 실행
        {
            Console.WriteLine("시간" + Environment.TickCount64);
            gameRoom.PushAfter(5 * 1000, Destroy);
            

            active = true;
        }
        else
        {
            CellPos += Speed * Program.ServerIntervalTick / 1000 * Dir;
        }

        Console.WriteLine("Arw" + CellPos + $"time {Environment.TickCount64}");

        var movePacket = new S_Move
        {
            ObjectId = Id,
            PositionInfo = PosInfo
        };

        gameRoom.BroadCast(CurrentRoomId, movePacket);
    }


    public override void Destroy()
    {
        if (Data == null || Data.projectile == null || OwnerId == -1 || gameRoom == null)
            Console.WriteLine("Destroy 오류");

        gameRoom.Push(() => { ObjectManager.Instance.Remove(Id); });
        gameRoom.Push(gameRoom.LeaveGame, Id);
        destroyed = true;
        Console.WriteLine("Destory");
    }

    public override GameObject GetOwner()
    {
        if (OwnerId == -1 || OwnerId == 0)
            return null;

        GameObject Owner;
        Owner = gameRoom.Map.FindObjById(CurrentRoomId, OwnerId); //데이터 레이스?
        return Owner;
    }

    public override void OnCollisionFeedback(GameObject other = null)
    {
        //특성에 따라 삭제 안될수도?
        Destroy();
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