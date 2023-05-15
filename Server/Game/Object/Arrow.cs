using System;
using Google.Protobuf.Protocol;

namespace Server.Game;

public class Arrow : Projectile
{
    public bool active;
    private bool Destoryed;

    public Arrow()
    {
        Console.WriteLine("Arrow");
    }

    public override void Update()
    {
        if (Data == null || Data.projectile == null || OwnerId == -1 || gameRoom == null || Destoryed)
            return;

        gameRoom.PushAfter(Program.ServerTick, Update);
        if (active == false) //처음 실행
        {
            Console.WriteLine("시간" + Environment.TickCount64);
            gameRoom.PushAfter(5 * 1000, Destroy);
            ;

            active = true;
        }
        else
        {
            CellPos += Speed * Program.ServerTick / 1000 * Dir;
        }

        Console.WriteLine("Arw" + CellPos + $"time {Environment.TickCount64}");

        var movePacket = new S_Move
        {
            ObjectId = Id,
            PositionInfo = PosInfo
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
        if (OwnerId == -1 || OwnerId == 0)
            return null;

        GameObject Owner;
        Owner = gameRoom.Map.FindObjById(CurrentRoomId, OwnerId); //데이터 레이스?
        return Owner;
    }

    public override void OnCollisionFeedback(GameObject other = null)
    {
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