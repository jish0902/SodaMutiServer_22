using System;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Session;
using ServerCore;

internal class PacketHandler
{
    internal static void C_LobbyInfoHandler(PacketSession session, IMessage packet)
    {
        var clientSession = (ClientSession)session;
        var lobbyInfoPakcet = (C_LobbyInfo)packet;

        clientSession.HandleLogin(lobbyInfoPakcet);
    }


    //------------------------------------------------ 인게임 시작
    public static void C_EnterGameHandler(PacketSession session, IMessage packet)
    {
        var clientSession = (ClientSession)session;
        var enterGamePakcet = (C_EnterGame)packet;

        clientSession.HandleEnterGame(enterGamePakcet);
    }


    public static void C_MoveHandler(PacketSession session, IMessage packet)
    {
        //Console.WriteLine($"x= {_packet.PositionInfo.PosX}y ={_packet.PositionInfo.PosY} z={_packet.PositionInfo.RotZ}");


        var clientSession = (ClientSession)session;
        var _packet = (C_Move)packet;


        var player = clientSession.MyPlayer; //나중에 null로 바꿔도 참조가능
        if (player == null)
            return;

        var room = player.gameRoom; //나중에 null로 바꿔도 참조가능
        if (room == null)
            return;

        room.Push(room.HandleMove, player, _packet);
    }

    internal static void C_HitHandler(PacketSession session, IMessage message)
    {
        var clientSession = (ClientSession)session;
        var hitpacket = (C_Hit)message;

        var player = clientSession.MyPlayer;
        if (player == null)
            return;

        var room = player.gameRoom;
        if (room == null)
            return;

        room.Push(room.HandleHit, player, hitpacket);
    }

    internal static void C_SkillHandler(PacketSession session, IMessage message)
    {
        var clientSession = (ClientSession)session;
        var skillPacket = (C_Skill)message;

        //Console.WriteLine($"{skillPacket.Info.SkillId}");
        var player = clientSession.MyPlayer;
        ;
        if (player == null)
            return;

        var room = player.gameRoom;
        if (room == null)
            return;

        room.Push(room.HandleSkill, player, skillPacket);

        //Console.WriteLine($"Skill{skillPacket.Info.SkillId}");
    }

    public static void C_RewardInfoHandler(PacketSession session, IMessage message)
    {
        var clientSession = (ClientSession)session;
        var info = message as C_RewardInfo;

        var t = (Define.RewardsType)info.RewardsType;

        var player = clientSession.MyPlayer;
        ;
        if (player == null)
            return;


        if (t == Define.RewardsType.LevelUp)
        {
            player.SetLevelUp();
        }
        else if (t == Define.RewardsType.RandomItem)
        {
            //Todo: 아이템 구현
        }
        else if (t == Define.RewardsType.RandomSpawn)
        {
            //Todo : 아군 몬스터 구현
        }
    }

    public static void C_PregameInfoHandler(PacketSession session, IMessage message)
    {
        var packet = (C_PregameInfo)message;
        var clientSession = (ClientSession)session;

        //clientSession.AccountDbId //TODO


        clientSession.LobbyGameInfo.ClassID = packet.Info.CharacterID;
        Console.WriteLine(clientSession.LobbyGameInfo.ClassID);
    }
}