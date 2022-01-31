using Google.Protobuf;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;



class PacketHandler
{
    internal static void S_LeaveGameHandler(PacketSession arg1, IMessage arg2)
    {
        throw new NotImplementedException();
    }

    internal static void S_EnterGameHandler(PacketSession arg1, IMessage arg2)
    {
        throw new NotImplementedException();
    }

    internal static void S_ConnectedHandler(PacketSession session , IMessage message)
    {
        Console.WriteLine("S_ConnectedHandlerS");
    }
}

