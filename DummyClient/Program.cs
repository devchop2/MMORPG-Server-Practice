using System.Net.Sockets;
using System.Net;
using System.Text;

namespace DummyClient
{
    public class Program
    {
        static void Main(string[] args)
        {
            string host = Dns.GetHostName(); // get host name;
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0]; //ip address : 부하분산을 위해 여러개의 ip가 생성될 수 있음. 우선 첫번 째 꺼를 사용함. 
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777); // 7777: port number

            //문지기
           
            while (true)
            {
                try
                {

                    Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    //문지기에게 입장문의
                    socket.Connect(endPoint);

                    Console.WriteLine("Connect To " + socket.RemoteEndPoint.ToString());

                    //send to server
                    byte[] sendBuff = Encoding.UTF8.GetBytes("Hello World!");
                    int sendBytes = socket.Send(sendBuff);

                    //receive from server
                    byte[] recvBuff = new byte[1024];
                    int recvBytes = socket.Receive(recvBuff);
                    string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);

                    Console.WriteLine("[From Server]" + recvData);

                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();

                    Thread.Sleep(1000);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception occurred." + e.Message);
                }
            }

        }

    }

}


