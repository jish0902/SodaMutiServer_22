using System;
using System.Net;
using Server.Session;
using ServerCore;
using Google.Protobuf.Protocol;
using System.Threading;
using Server.Game;
using Server.Data;
using System.Collections.Generic;

namespace Server
{

    // 만드는중


    

    class Program
    {
        public const int ServerTick = 200; //0.2sec
        static Listener _listener = new Listener();

        public static string IpAddress { get; set; }


        static void GameLogicTask()
        {
            while (true)
            {
                GameLogic.Instance.Update();
                Thread.Sleep(0);
            }
        }

        static void NetworkTask()
        {
            while (true)
            {
                foreach (ClientSession session in SessionManager.Instance.GetSessions())
                {
                    session.FlushSend();
                }
            }
        }

        

        static void Main(string[] args)
        {

            Console.WriteLine("1");
            ConfingManager.LoadConfig();
            DataManager.LoadData();
            
            GameLogic.Instance.Push(() => { GameLogic.Instance.Add(21); }); //방하나 추가

            

            // DNS (Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            
            foreach (var _ipAddress in ipHost.AddressList)
            {
                if (_ipAddress.ToString().Contains("192") || _ipAddress.ToString().Contains("218") || _ipAddress.ToString().Contains("172"))
                {
                    ipAddr = _ipAddress;
                    //ipAddr = IPAddress.Any;
                        
                    break;
                }
            }

            
           
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            IpAddress = ipAddr.ToString();


            _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
            Console.WriteLine("Listening...");

            //NetworkTask
            {
                Thread t = new Thread(NetworkTask);
                t.Name = "NetworkTask";
                t.Start();
            }

            //GameLogic
            Thread.CurrentThread.Name = "GameLogic";
            GameLogicTask();

        }
    }
}
