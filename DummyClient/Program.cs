using System.Net;
using System.Threading;
using DummyClient.Session;
using ServerCore;

namespace DummyClient
{
    internal class Program
    {
        public static string IpAddress { get; set; }

        private static void Main(string[] args)
        {
            // DNS (Domain Name System)
            var host = Dns.GetHostName();
            var ipHost = Dns.GetHostEntry(host);
            var ipAddr = ipHost.AddressList[1];
            var endPoint = new IPEndPoint(ipAddr, 7777);

            IpAddress = ipAddr.ToString();


            var connector = new Connector();
            connector.Connect(endPoint, () => { return new ServerSession(); });


            while (true) Thread.Sleep(10000);
        }
    }
}