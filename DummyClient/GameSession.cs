using ServerCore;
using System.Net;
using System.Text;

namespace DummyClient
{
    public class Packet
    {
        public ushort size;
        public int packetId;

    }

    public class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine("On Connected :"+endPoint);

            Packet testPacket = new Packet() { size = 4, packetId = 7 };
            ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);

            byte[] buffer1 = BitConverter.GetBytes(testPacket.size);
            byte[] buffer2 = BitConverter.GetBytes(testPacket.packetId);
            Array.Copy(buffer1, 0, openSegment.Array, openSegment.Offset, buffer1.Length);
            Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer1.Length, buffer2.Length);

            ArraySegment<byte> sendBuff = SendBufferHelper.Close(testPacket.size);
            Send(sendBuff);

            Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine("OnDisconnected :" + endPoint);
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Server] {recvData}\n");
            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine("Send Complete. total Bytes:" + numOfBytes);
        }
    }
}
