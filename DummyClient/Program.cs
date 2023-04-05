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
            IPAddress ipAddr = ipHost.AddressList[0]; //ip address : 부하분산을 위해 여러개의 ip가 생성될 수 있음. 우선 첫번 째 꺼를 사용함. 
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777); // 7777: port number

            //서버에게 연결을 시도. 
            Connector connector = new Connector();
            connector.Connect(endPoint, () => { return SessionManager.Instance.Generate(); }, 10);

            while (true)
            {
                SessionManager.Instance.SendForEach();
                Thread.Sleep(250);
            }

        }

    }

}


