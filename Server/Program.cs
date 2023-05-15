using System;
using System.Collections.Generic;
using System.Net;
using System.Numerics;
using System.Threading;
using Server.Data;
using Server.Game;
using Server.Session;
using ServerCore;

namespace Server;

// 만드는중

internal class Program
{
    public const int ServerTick = 200; //0.2sec
    private static readonly Listener _listener = new();

    public static string IpAddress { get; set; }


    private static void GameLogicTask()
    {
        while (true)
        {
            GameLogic.Instance.Update();
            Thread.Sleep(0);
        }
    }

    private static void NetworkTask()
    {
        while (true)
            foreach (var session in SessionManager.Instance.GetSessions())
                session.FlushSend();
    }


    private static void Main(string[] args)
    {
        /*
        Circle GetBounds(GameObject obj)
        {
            return new Circle(new Vector2(obj.PosInfo.PosX, obj.PosInfo.PosY), Math.Max(0.5f, 0.5f));
        }

        QuadTree<GameObject> _quadTree;

        _quadTree = new QuadTree<GameObject>(
            -100, // yep, MAX
            -100, // MIN
            200,
            200
        );

        var p = new Player
        {
            PosInfo = { PosX = 1, PosY = 4},
            Id = 1
        };

        _quadTree.Insert(p, GetBounds(p));

        var p2 = new Player
        {
            Id = 2,
            PosInfo = { PosX = 1, PosY = 1 }
        };

        _quadTree.Insert(p2, GetBounds(p2));

        var nearest = new List<GameObject>(_quadTree.GetNodesInside(new Vector2(0, 0), 4));
        foreach (var obj in nearest)
        {
            Console.WriteLine(obj.info.PositionInfo);
        }
*/

       

        Console.WriteLine("1");
        ConfingManager.LoadConfig();
        DataManager.LoadData();

        GameLogic.Instance.Push(() => { GameLogic.Instance.Add(21); }); //방하나 추가


        // DNS (Domain Name System)
        var host = Dns.GetHostName();
        var ipHost = Dns.GetHostEntry(host);
        var ipAddr = IPAddress.Loopback;


        /*
        foreach (var _ipAddress in ipHost.AddressList)
        {
            if (_ipAddress.AddressFamily == AddressFamily.)
            {
                ipAddr = _ipAddress;
            }
        }
        */


        Console.WriteLine(ipAddr);

        var endPoint = new IPEndPoint(ipAddr, 7777);

        IpAddress = ipAddr.ToString();


        _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
        Console.WriteLine("Listening...");

        //NetworkTask
        {
            var t = new Thread(NetworkTask);
            t.Name = "NetworkTask";
            t.Start();
        }

        //GameLogic
        Thread.CurrentThread.Name = "GameLogic";
        GameLogicTask();
        
    }
}