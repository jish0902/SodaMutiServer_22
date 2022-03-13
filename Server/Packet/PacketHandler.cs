﻿using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Game;
using Server.Session;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{

    internal static void C_LobbyInfoHandler(PacketSession session, IMessage packet)
    {
        ClientSession clientSession = (ClientSession)session;
        C_LobbyInfo lobbyInfoPakcet = (C_LobbyInfo)packet;

        clientSession.HandleLogin(lobbyInfoPakcet);
    }



    //------------------------------------------------ 인게임 시작
    public static void C_EnterGameHandler(PacketSession session, IMessage packet)
    {
        ClientSession clientSession = (ClientSession)session;
        C_EnterGame enterGamePakcet = (C_EnterGame)packet;

        clientSession.HandleEnterGame(enterGamePakcet);

    }


    public static void C_MoveHandler(PacketSession session, IMessage packet)
    {
        ClientSession clientSession = (ClientSession)session;
        C_Move _packet = (C_Move)packet;

        Console.WriteLine($"x= {_packet.PositionInfo.PosX}y ={_packet.PositionInfo.PosY}");

        Player player = clientSession.MyPlayer; //나중에 null로 바꿔도 참조가능
        if (player == null)
            return;

        GameRoom room = player.Room; //나중에 null로 바꿔도 참조가능
        if (room == null)
            return;

        room.Push(room.HandleMove, player, _packet);
    }

    internal static void C_SkillHandler(PacketSession session, IMessage message)
    {
        ClientSession clientSession = (ClientSession)session;
        C_Skill skillPacket = (C_Skill)message;

        Player player = clientSession.MyPlayer; ;
        if (player == null)
            return;

        GameRoom room = player.Room;
        if (room == null)
            return;

        room.Push(room.HandleSkill, player, skillPacket);

    }
}

