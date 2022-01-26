using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    public class Connector
    {
        
        Func<Session> _sessionFactory;

        public void Connect(IPEndPoint iPEndPoint, Func<Session> sessionFactory,int number = 1)
        {

            for (int i = 0; i < number; i++)
            {
                Socket Socket = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _sessionFactory = sessionFactory;

                
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += OnConnectCompleted;
                args.RemoteEndPoint = iPEndPoint;
                args.UserToken = Socket;

                RegisterConnect(args);

                //temp
                Thread.Sleep(10);
            }

            //args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);

        }

        public void RegisterConnect(SocketAsyncEventArgs args)
        {
            Socket socket = (Socket)args.UserToken;
            if (socket == null)
                return;

            try
            {
                bool pending = socket.ConnectAsync(args);
                if (pending == false)
                    OnConnectCompleted(null, args);
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }

        }


        public void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
        {
            try
            {
                if (args.SocketError == SocketError.Success)
                {
                    Session session = _sessionFactory.Invoke();
                    session.Start(args.ConnectSocket);
                    session.OnConnected(args.ConnectSocket.RemoteEndPoint);
                }
                else
                    Console.WriteLine(args.SocketError.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}
