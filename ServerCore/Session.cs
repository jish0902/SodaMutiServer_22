using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServerCore
{
   
    
    public abstract class PacketSession : Session
    {
        public static readonly int HeaderSize = 2;
        // [size(2)][packetId(2)][ ... ][size(2)][packetId(2)][ ... ]

        public sealed override int OnRecv(ArraySegment<byte> buffer)
        {
            int processLen = 0;

            while (true)
            {
                //해드 크기
                if (buffer.Count < HeaderSize)
                    break;

                //전체 크기
                ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset); //ushort
                if (buffer.Count < dataSize)
                    break;

                //적어도 1개의 페킷 완성
                OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));

                processLen += dataSize;
                buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);

            }
            return processLen;

        }

        public abstract void OnRecvPacket(ArraySegment<byte> buffer);

    }




    public abstract class Session
    {
        Socket _socket;
        int _disconnected = 0;

        RecvBuffer _recvBuffer = new RecvBuffer(65535);

        object _lock = new object();
        Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();

        public abstract void OnConnected(EndPoint endPoint);
        public abstract int OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfByte);
        public abstract void OnDisconnected(EndPoint endPoint);

        void Clear()
        {
            lock (_lock)
            {
                _sendQueue.Clear();
                _pendingList.Clear();
            }
        }

        public void Start(Socket socket)
        {
            _socket = socket;

            _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

            RegisterRecv();
        }
        
        public void Send(List<ArraySegment<byte>> sendBuffList)
        {
            if (sendBuffList.Count == 0)
                return;

            lock (_lock)
            {
                foreach (ArraySegment<byte> sendBuff in sendBuffList)
                    _sendQueue.Enqueue(sendBuff);

                if (_pendingList.Count == 0)
                    RegisterSend();
            }
        }

        public void Send(ArraySegment<byte> sendBuff)
        {
            if (sendBuff.Count == 0)
                return;

            lock (_lock)
            {
                _sendQueue.Enqueue(sendBuff);
                if (_pendingList.Count == 0)
                    RegisterSend();
            }
        }

        public void Disconnect()
        {
            if(Interlocked.Exchange(ref _disconnected,1) == 1)
            {
                return;
            }

            OnDisconnected(_socket.RemoteEndPoint);
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();

            Clear();
        }

        #region 네트워크 통신

        void RegisterSend()
        {
            if (_disconnected == 1)
                return;

            while(_sendQueue.Count > 0)
            {
                ArraySegment<byte> buff = _sendQueue.Dequeue();
                _pendingList.Add(buff);
            }

            _sendArgs.BufferList = _pendingList;

            try
            {
                bool pending = _socket.SendAsync(_sendArgs);
                if (pending == false)
                    OnSendCompleted(null, _sendArgs);
            }
            catch (Exception e)
            {

                Console.WriteLine($"RegisterSend Failed {e}");
            }

        }


        void OnSendCompleted(object sender ,SocketAsyncEventArgs args)
        {

            lock (_lock)
            {
                if(args.SocketError == SocketError.Success && args.BytesTransferred > 0)
                {
                    try
                    {
                        args.BufferList = null;
                        _pendingList.Clear();

                        OnSend(_sendArgs.BytesTransferred);

                        if (_sendQueue.Count > 0)
                            RegisterSend();

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"OnSendCompleted Failed {e}\n");
                    }
                }
                else
                {
                    Disconnect();
                }
            }
        }


        void RegisterRecv()
        {
            if (_disconnected == 1)
                return;

            _recvBuffer.Clean();

            ArraySegment<byte> segment = _recvBuffer.WriteSegment;
            _recvArgs.SetBuffer(segment.Array,segment.Offset,segment.Count);

            try
            {
                bool pending = _socket.ReceiveAsync(_recvArgs);
                if (pending == false)
                    OnRecvCompleted(null, _recvArgs);

            }
            catch (Exception e)
            {
                Console.WriteLine($"RegisterRecv Failed {e}");

            }
        }


        void OnRecvCompleted(object Sender, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                if (args.SocketError == SocketError.Success && args.BytesTransferred > 0)
                {
                    try
                    {
                        //wirte커서 이동
                        if(_recvBuffer.OnWrite(_recvArgs.BytesTransferred) == false)
                        {
                            Disconnect();
                            return;
                        }

                        //데이터 컨텐츠에 넘겨줌
                        int processLen = OnRecv(_recvBuffer.ReadSegment);
                        if (processLen < 0 || _recvBuffer.DataSize < processLen)
                        {
                            Disconnect();
                            return;
                        }
                        //read 커서 이동
                        if(_recvBuffer.OnRead(processLen) == false)
                        {
                            Disconnect();
                            return;
                        }

                        RegisterRecv();

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"OnSendCompleted Failed {e}");
                    }
                }
                else
                {
                    Disconnect();
                }
            }
        }


        #endregion




    }
}
