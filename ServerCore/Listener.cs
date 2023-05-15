using System;
using System.Net;
using System.Net.Sockets;

namespace ServerCore;

public class Listener
{
    private Socket _listenSocket;
    private Func<Session> _sessionFactory;

    public void Init(IPEndPoint iPEndPoint, Func<Session> sessionFactory, int register = 10, int backlog = 10)
    {
        _listenSocket = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _sessionFactory = sessionFactory;

        // 문지기 교육
        _listenSocket.Bind(iPEndPoint);

        //영업 시작
        _listenSocket.Listen(backlog);

        for (var i = 0; i < register; i++)
        {
            var args = new SocketAsyncEventArgs();
            args.Completed += OnAcceptCompleted;
            RegisterAccept(args);
        }

        //args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
    }

    public void RegisterAccept(SocketAsyncEventArgs args)
    {
        args.AcceptSocket = null;
        try
        {
            var pending = _listenSocket.AcceptAsync(args);
            if (pending == false)
                OnAcceptCompleted(null, args);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }


    public void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
    {
        try
        {
            if (args.SocketError == SocketError.Success)
            {
                var session = _sessionFactory.Invoke();
                session.Start(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
            }
            else
            {
                Console.WriteLine(args.SocketError.ToString());
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("OnAcceptcompleted error");
        }

        RegisterAccept(args);
    }
}