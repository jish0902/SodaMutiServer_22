using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Session;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{
    internal static void C_MoveHandler(PacketSession arg1, IMessage arg2)
    {
        ClientSession session = (ClientSession)arg1;
        C_Move pakcet = (C_Move)arg2;

        Console.WriteLine($"x= {pakcet.PositionInfo.PosX}y ={pakcet.PositionInfo.PosY}");

    }
}

