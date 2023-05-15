using System.Collections.Generic;
using Google.Protobuf.Protocol;
using Server.Data;
using Server.Game;
using Server.Game.Utils;
using ServerCore;

namespace Server.Session;

public class PreGameInfo
{
    public int ClassID;
    //스킬 종류
    //룬 같은거
    //스킨 여부
}

public partial class ClientSession : PacketSession
{
    public int AccountDbId { get; private set; }
    public List<PlayerProfile> LobbyPlayers { get; set; } = new();
    public PreGameInfo LobbyGameInfo { get; set; } = new();

    // 구글 토큰 인증
    //HandleLogin
    // LobbyPlayers.add (PlayerProfile);
    public void HandleLogin(C_LobbyInfo message)
    {
        //message.DeviceId;
        //찾아보고

        var lobbyPaket = new S_LobbyPlayerInfo();
        lobbyPaket.Profile = new PlayerProfile
        {
            PlayerDbId = 10,
            Name = "jish", //나중에 강한것은 이름앞에 칭호가 붙어서 길어지는 느낌으로
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
            MyPlayer.CurrentRoomId = -1;
            MyPlayer.info.Name = enterGamePacket.Name + SessionId;
            MyPlayer.info.PositionInfo.State = CreatureState.Idle;
            MyPlayer.info.PositionInfo.PosX = 0;
            MyPlayer.info.PositionInfo.PosY = 0;


            StatInfo info;
            if (DataManager.StatDict.TryGetValue(Utills.GetStatFormClass(LobbyGameInfo.ClassID), out info))
            {
                MyPlayer.info.StatInfo = info;
                MyPlayer.stat.MergeFrom(info);
            }
        }
        //ServerState = PlayerServerState.ServerStateGame;


        GameLogic.Instance.Push(() =>
        {
            var room = GameLogic.Instance.Find(1);
            room.Push(room.EnterGame, MyPlayer, true);
        });
    }
}