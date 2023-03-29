using ServerCore;
using System;
using System.Net;
using System.Text;

namespace DummyClient
{
    public abstract class Packet
    {
        public ushort size;
        public int packetId;

        public abstract ArraySegment<byte> Serialize();
        public abstract void Deserialize(ArraySegment<byte> data);
       
    }

    public enum PacketID 
    {  
        PlayerInfoReq = 1,
        PlayerInfoOk = 2 
    }

    class PlayerInfoReq : Packet
    {
        public long playerId;

        public PlayerInfoReq()
        {
            packetId = (ushort)PacketID.PlayerInfoReq;
        }

        public override ArraySegment<byte> Serialize()
        {
            ArraySegment<byte> s = SendBufferHelper.Open(4096);

            ushort count = 0;
            bool success = true;

            count += sizeof(ushort); //size

            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), packetId);
            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), playerId);
            count += sizeof(long);

            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count - count), count);
            //패킷사이즈를 계산해서 마지막에 넣어줌. 대신, 첫번째 위치에 넣어줘야함.

            return  SendBufferHelper.Close(count);
        }
        public override void Deserialize(ArraySegment<byte> data)
        {
            throw new NotImplementedException();
        }
    }

    public class ServerSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine("On Connected :"+endPoint);

            PlayerInfoReq testPacket = new PlayerInfoReq() {packetId = (ushort)PacketID.PlayerInfoReq, playerId = 1001 };
            var sendBuff = testPacket.Serialize();

            //var sendStr = BitConverter.ToString(sendBuff.Array, sendBuff.Offset, sendBuff.Count); 
            //Console.WriteLine("Send:"+sendStr);

            Send(sendBuff);
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
