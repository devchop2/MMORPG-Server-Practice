using System.Net;
using System.Text;
using ServerCore;

namespace Server
{
    public class Packet
    {
        public ushort size;
        public int packetId;
    }

    public enum PacketID
    {
        PlayerInfoReq = 1,
        PlayerInfoOk = 2
    }
    class PlayerInfoReq : Packet
    {
        public long playerId;
    }

    class PlayerInfoOk : Packet
    {
        public int hp;
        public int attack;
    }

    public  class ClientSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine("On Connected :" + endPoint);
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine("OnDisconnected :" + endPoint);
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = BitConverter.ToString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Server] {recvData}\n");

            ushort count = 0;
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            count += sizeof(ushort);
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += sizeof(ushort);
               

            switch ((PacketID)id)
            {
                case PacketID.PlayerInfoReq:
                    {
                        //todo;
                        long playerId = BitConverter.ToInt64(buffer.Array, buffer.Offset + count);
                        count += sizeof(long);
                        Console.WriteLine("[playerId]:"+playerId);
                        break;
                    }
                case PacketID.PlayerInfoOk:
                    {
                        //todo;
                        break;
                    }
            }
            
            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine("Send Complete. total Bytes:" + numOfBytes);
        }
    }
}
