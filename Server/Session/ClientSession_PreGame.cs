using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Game;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Session
{
    public partial class ClientSession : PacketSession
    {
        public int  AccountDbId { get; private set; }
        public List<PlayerProfile> LobbyPlayers { get; set; } = new List<PlayerProfile>();

        // 구글 토큰 인증
        //HandleLogin
        // LobbyPlayers.add (PlayerProfile);
        public void HandleLogin(C_LobbyInfo message)
        {
            //message.DeviceId;
            //찾아보고

            S_LobbyPlayerInfo lobbyPaket = new S_LobbyPlayerInfo();
            lobbyPaket.Profile = new PlayerProfile()
            {
                PlayerDbId = 10,
                Name = "jish",
                CharacterID = { 3, 12, 10 },
                Coins = { 10, 20, 30 }
            };

            Send(lobbyPaket);
        }


        public void HandleEnterGame(C_EnterGame enterGamePacket)
        {
            //if (ServerState != PlayerServerState.ServerStateLobby)
            //    return;


            //PlayerProfile playerInfo = LobbyPlayers.Find(p => p.Name == enterGamePacket.Name);


            MyPlayer = ObjectManager.Instance.Add<Player>();
            {
                MyPlayer.Session = this;
                MyPlayer.CurrentPlanetId = -1;
                MyPlayer.info.Name = enterGamePacket.Name+ SessionId.ToString();
                MyPlayer.info.PositionInfo.State = CreatureState.Idle;
                MyPlayer.info.PositionInfo.PosX = 0;
                MyPlayer.info.PositionInfo.PosY = 0;
                MyPlayer.Side = 0;

                StatInfo info;
                if(Data.DataManager.StatDict.TryGetValue(1001,out info) == true)
                {
                    MyPlayer.info.StatInfo = info;
                    MyPlayer.stat.MergeFrom(info);
                }
            }
            //ServerState = PlayerServerState.ServerStateGame;

            GameLogic.Instance.Push(() =>
            {
                GameRoom room = GameLogic.Instance.Find(1);
                room.Push(room.EnterGame, MyPlayer, true);
            });
        }
    }
}
