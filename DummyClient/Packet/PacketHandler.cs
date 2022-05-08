using Google.Protobuf;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;



class PacketHandler
{
    internal static void S_LeaveGameHandler(PacketSession arg1, IMessage arg2)
    {
        Console.WriteLine("S_LeaveGameHandler");

    }

    internal static void S_EnterGameHandler(PacketSession arg1, IMessage arg2)
    {
        Console.WriteLine("S_EnterGameHandler");
    }

    internal static void S_ConnectedHandler(PacketSession session , IMessage message)
    {
        Console.WriteLine("S_ConnectedHandlerS");
    }

    internal static void S_SpawnHandler(PacketSession arg1, IMessage arg2)
    {
        Console.WriteLine("S_SpawnHandler");

    }

    internal static void S_DespawnHandler(PacketSession arg1, IMessage arg2)
    {
        Console.WriteLine("S_DespawnHandler");

    }

    internal static void S_MoveHandler(PacketSession arg1, IMessage arg2)
    {
        Console.WriteLine("S_MoveHandler");

    }

    internal static void S_ChangeHpHandler(PacketSession arg1, IMessage arg2)
    {
        Console.WriteLine("S_ChangeHpHandler");

    }

    internal static void S_DieHandler(PacketSession arg1, IMessage arg2)
    {
        Console.WriteLine("S_DieHandler");

    }


    internal static void S_LobbyPlayerInfoHandler(PacketSession arg1, IMessage arg2)
    {
        Console.WriteLine("S_LobbyPlayerInfoHandler");

    }

    internal static void S_SkillHandler(PacketSession arg1, IMessage arg2)
    {
        Console.WriteLine("S_SkillHandler");

    }

    internal static void S_StatChangeHandler(PacketSession arg1, IMessage arg2)
    {
        throw new NotImplementedException();
    }

    internal static void S_RoomInfoHandler(PacketSession arg1, IMessage arg2)
    {
        throw new NotImplementedException();
    }
}

