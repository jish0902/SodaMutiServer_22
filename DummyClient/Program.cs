using System;
using System.Net;
using System.Threading;
using DummyClient.Session;
using ServerCore;

namespace DummyClient
{
    class Program
    {

        public static string IpAddress { get; set; }

        static void Main(string[] args)
        {
           

            // DNS (Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[1];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            IpAddress = ipAddr.ToString();

            
            Connector connector = new Connector();
            connector.Connect(endPoint,() => { return new ServerSession(); }, 1);




            while (true)
            {
                Thread.Sleep(10000);
            }

        }
    }
}
