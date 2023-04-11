using ServerCore;
using System.Net;

namespace Server
{
    public class Program
    {
        static Listener _listener = new Listener();
        public static GameRoom room = new GameRoom();

        static void FlushRoom()
        {
            room.Push(() => room.Flush());
            JobTimer.Instance.Push(FlushRoom, 250);
        }

        static void Main(string[] args)
        {
            try
            {
                //문지기 정보
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

                //문지기 교육(초기화) => 작업은 Listener.cs 가 알아서해줌. 소켓연결되면 GameSession으로 만들어서 Start()까지 자동으로해줌.
                _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });

                FlushRoom();
                while (true)
                {
                    Thread.Sleep(100);
                    JobTimer.Instance.Flush();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception occurred. msg:" + e.Message);
            }
        }
    }
}