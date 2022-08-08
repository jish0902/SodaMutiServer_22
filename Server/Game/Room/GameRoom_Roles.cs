using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public partial class GameRoom 
    {
		bool occupationMode = false;

		public void SetGameMode()
		{
			occupationMode = true;
		}


		public void UpdateGameRole()
		{
			if (occupationMode)
			{
				List<Player> _players = Map.GetPlayerInOccupationPos(2); //추가

				HashSet<int> rooms = new HashSet<int>();
				foreach (Player player in _players)
                {
					if(player != null)
						rooms.Add(player.CurrentRoomId);
				}

                foreach (int rId in rooms)
                {
                    Room room = Map.GetRoom(rId);
                    if (room.Owner.y % 5 == 0)
                    {
                        S_RoomInfo roomPacket = new S_RoomInfo();
                        roomPacket.RoomInfos.Add(new RoomInfo()
                        {
                            RoomId = room.Id,
                            OwnerId = room.Owner.x,
                            TryOwnerId = room.TryOwnerId,
                            TryOwnerValue = ((float)room.Owner.y / Room.rOwnerValInitCount),
                        });
                        Console.WriteLine($"{roomPacket.RoomInfos[0].RoomId} , " +
                            $"{roomPacket.RoomInfos[0].OwnerId} , " +
                            $"{roomPacket.RoomInfos[0].TryOwnerId} , " +
                            $"{roomPacket.RoomInfos[0].TryOwnerValue} , ");
                        BroadCast(rId, roomPacket);
                    }


                }//loop

            }//occupationMode End














        }


	}
}
