using System.Net;
using System.Text;

namespace ServerCore
{
    public class Kinght 
    {
        public int hp;
        public int attack;
    }

    public class GameSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Thread.Sleep(5000);
            Disconnect();
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);
            Console.WriteLine($"RecvPacket. id:${id}, dataSize:{dataSize}");

        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine("Send Complete. total Bytes:" + numOfBytes);
        }
        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine("OnDisconnected :"+endPoint);
        }

      
    }
}
