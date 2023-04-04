using ServerCore;
using System.Net;

namespace Server
{
    public class Program
    {
        static Listener _listener = new Listener();
        static void Main(string[] args)
        {
            PacketManager.Instance.Register();
            try
            {
                //문지기 정보
                string host = Dns.GetHostName(); // get host name;
                IPHostEntry ipHost = Dns.GetHostEntry(host);
                IPAddress ipAddr = ipHost.AddressList[0]; //ip address : 부하분산을 위해 여러개의 ip가 생성될 수 있음. 우선 첫번 째 꺼를 사용함. 
                IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777); // 7777: port number

                //문지기 교육(초기화) => 작업은 Listener.cs 가 알아서해줌. 소켓연결되면 GameSession으로 만들어서 Start()까지 자동으로해줌.
                _listener.Init(endPoint, () => { return new ClientSession(); });
                _listener.RegisterAccept();

                while (true)
                {
                    //메인스레드가 죽지 않기 위해 계속 돌아가는중....!
                    Thread.Sleep(100);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception occurred. msg:" + e.Message);
            }
        }
    }
}