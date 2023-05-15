using System;
using System.Collections.Generic;
using System.Net;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Game;
using ServerCore;

namespace Server.Session;

public partial class ClientSession : PacketSession
{
    private long _lastSendTick;
    private readonly object _lock = new();

    private long _pingpongTick;

    //패킷 모아보내기
    private int _reservedSendBytes;
    private List<ArraySegment<byte>> _reserveQueue = new();
    public Player MyPlayer { get; set; }
    public int SessionId { get; set; }

    public PlayerServerState ServerState { get; } = PlayerServerState.ServerStateLogin;

    public void Ping()
    {
        if (_pingpongTick > 0)
        {
            var delta = Environment.TickCount64 - _pingpongTick;
            if (delta > 30 * 1000) //30초
            {
                Console.WriteLine("Disconnected by PingCheak");
                Disconnect();
            }
        }
    }


    public void HandlePong()
    {
        _pingpongTick = Environment.TickCount64;
    }


    #region Network

    public void Send(IMessage packet)
    {
        var msgName = packet.Descriptor.Name.Replace("_", string.Empty);
        var msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);

        var size = (ushort)packet.CalculateSize();
        var sendBuffer = new byte[size + 4];
        Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
        Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, sizeof(ushort), sizeof(ushort));
        Array.Copy(packet.ToByteArray(), 0, sendBuffer, 2 * sizeof(ushort), size);

        lock (_lock)
        {
            _reserveQueue.Add(sendBuffer);
            _reservedSendBytes += sendBuffer.Length;
        }
    }

    //실제 network IO 처리하는 부분
    public void FlushSend()
    {
        List<ArraySegment<byte>> sendList = null;
        lock (_lock)
        {
            var delta = Environment.TickCount64 - _lastSendTick;
            if (delta < 50 && _reservedSendBytes < 10000)
                return;

            _lastSendTick = Environment.TickCount64;
            _reservedSendBytes = 0;

            sendList = _reserveQueue;
            _reserveQueue = new List<ArraySegment<byte>>();
        }

        Send(sendList);
    }


    public override void OnConnected(EndPoint endPoint)
    {
        {
            var connectedPacket = new S_Connected();

            Send(connectedPacket);
        }

        //GameLogic.Instance.PushAfter(5000, Ping);
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        GameLogic.Instance.Push(() =>
        {
            if (MyPlayer == null)
                return;

            var room = GameLogic.Instance.Find(1);
            room.Push(room.LeaveGame, MyPlayer.info.ObjectId);
        });
        SessionManager.Instance.Remove(this);

        Console.WriteLine("OnDisconnected");
    }

    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        PacketManager.Instance.OnRecvPacket(this, buffer);
    }

    public override void OnSend(int numOfByte)
    {
        //
        //
        //
        //Console.WriteLine(numOfByte);
    }

    #endregion
}