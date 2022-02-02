using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Google.Protobuf.Protocol;
using Google.Protobuf;

namespace Server.Session
{
    public partial class ClientSession : PacketSession
    {
        //public Player MyPlayer { get; set; }
        public int SessionId { get; set; }
        object _lock = new object();
        List<ArraySegment<byte>> _reserveQueue = new List<ArraySegment<byte>>();

        //public PlayerServerState ServerState { get; private set; } = PlayerServerState.ServerStateLogin;

        long _pingpongTick = 0;

        //패킷 모아보내기
        int _reservedSendBytes = 0;
        long _lastSendTick = 0;

        public void Ping()
        {
            if(_pingpongTick > 0)
            {
                long delta = Environment.TickCount64 - _pingpongTick;
                if(delta > 30 * 1000) //30초
                {
                    Console.WriteLine("Disconnected by PingCheak");
                    Disconnect();
                    return;

                }
                
            }

        }
        public void HandlePong()
        {
            _pingpongTick = System.Environment.TickCount64;
        }


        

        #region Network

        public void Send(IMessage packet)
        {
            string msgName = packet.Descriptor.Name.Replace("_", string.Empty);
            MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName); 

            ushort size = (ushort)packet.CalculateSize();
            byte[] sendBuffer = new byte[size + 4];
            Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
            Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, sizeof(ushort), sizeof(ushort));
            Array.Copy(packet.ToByteArray(), 0, sendBuffer,  2 * sizeof(ushort), size);
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
				long delta = (System.Environment.TickCount64 - _lastSendTick);
                if (delta < 50 && _reservedSendBytes < 10000)
                    return;

                _lastSendTick = System.Environment.TickCount64;
                _reservedSendBytes = 0;

                sendList = _reserveQueue;
                _reserveQueue = new List<ArraySegment<byte>>();
                
            }
            Send(sendList);
        }



        public override void OnConnected(EndPoint endPoint)
        {
            {
                S_Connected connectedPacket = new S_Connected();
                
                Send(connectedPacket);
            }
            
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
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
}
