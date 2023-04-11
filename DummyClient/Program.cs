using System.Net.Sockets;
using System.Net;
using System.Text;
using ServerCore;

namespace DummyClient
{
    public class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(1000);
            string host = Dns.GetHostName(); // get host name;
            IPHostEntry ipHost = Dns.GetHostEntry(host);

            IPAddress ipAddr = ipHost.AddressList[0];
            foreach (var item in ipHost.AddressList)
            {
                if (item.ToString().StartsWith("172."))
                {
                    ipAddr = item;
                    break;
                }
            }

            IPEndPoint endPoint = new IPEndPoint(ipAddr, 8080); // 7777: port number

            //서버에게 연결을 시도. 
            Connector connector = new Connector();
            connector.Connect(endPoint, () => { return SessionManager.Instance.Generate(); }, 1);

            while (true)
            {
                SessionManager.Instance.SendForEach();
                Thread.Sleep(250);
            }

        }

    }

}


