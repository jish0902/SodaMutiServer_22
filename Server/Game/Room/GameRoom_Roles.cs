using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Protocol;

namespace Server.Game;

public partial class GameRoom
{
    private bool occupationMode;

    public void SetGameMode()
    {
        occupationMode = true;
    }


    public void UpdateGameRole()
    {
        if (occupationMode)
        {
            var _players = Map.AddPlayerInOccupationPos(); //추가
            
            var rooms = new HashSet<int>(_players
                .Where(p => p != null)
                .Select(p => p.CurrentRoomId));


            foreach (var rId in rooms) //패킷 전송
            {
                var room = Map.GetRoom(rId);
                if (room.Owner.y % 5 == 0)
                    return;
                
                var roomPacket = new S_RoomInfo();
                roomPacket.RoomInfos.Add(new RoomInfo
                {
                    RoomId = room.Id,
                    OwnerId = room.Owner.x,
                    TryOwnerId = room.TryOwnerId,
                    TryOwnerValue = (float)room.Owner.y / Room.rOwnerValInitCount
                });
                
                Console.WriteLine($"{roomPacket.RoomInfos[0].RoomId} , " +
                                  $"{roomPacket.RoomInfos[0].OwnerId} , " +
                                  $"{roomPacket.RoomInfos[0].TryOwnerId} , " +
                                  $"{roomPacket.RoomInfos[0].TryOwnerValue} , ");
                BroadCast(rId, roomPacket);
                
            } //loop
        } //occupationMode End
    }
}