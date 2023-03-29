using System.Net;
using System.Text;
using ServerCore;

namespace Server
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
        public string name; //가변길

        public PlayerInfoReq()
        {
            packetId = (ushort)PacketID.PlayerInfoReq;
        }

        public override ArraySegment<byte> Serialize()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

            ushort count = 0;
            bool success = true;

            count += sizeof(ushort); //size

            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), packetId); ;
            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), playerId);
            count += sizeof(long);

            //send string [size][data]
            ushort nameLen = (ushort)Encoding.Unicode.GetByteCount(name);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
            count += sizeof(ushort);

            Array.Copy(Encoding.Unicode.GetBytes(name), 0, segment.Array, count, nameLen);
            count += nameLen;

            //write size
            success &= BitConverter.TryWriteBytes(s, count);

            // 클라이언트가 그짓말할거라는거 생각해야 함 
            //패킷사이즈를 계산해서 마지막에 넣어줌. 대신, 첫번째 위치에 넣어줘야함.

            return SendBufferHelper.Close(count);
        }

        public override void Deserialize(ArraySegment<byte> data)
        {

            ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(data.Array, data.Offset, data.Count);
            ushort count = 0;
            //ushort size = BitConverter.ToUInt16(data.Array, data.Offset);
            count += sizeof(ushort);
            //ushort id = BitConverter.ToUInt16(data.Array, data.Offset + count);
            count += sizeof(ushort);

            //playerId = BitConverter.ToInt64(data.Array, data.Offset + count);
            playerId = BitConverter.ToInt64(s.Slice(count, s.Length - count));  //좀더 안전한 방법을 사용 . 데이터가 long 사이즈만큼 없을수도있음.
            count += sizeof(long);
            Console.WriteLine("[playerId]:" + playerId);

            //read string
            ushort nameLen = (ushort)BitConverter.ToInt16(s.Slice(count, s.Length - count));

            count += sizeof(ushort);
            name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
            Console.WriteLine($"[PlayerName] {name}");
        }
    }


    public class ClientSession : Session
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
            ushort packetId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += sizeof(ushort);


            switch ((PacketID)packetId)
            {
                case PacketID.PlayerInfoReq:
                    {
                        PlayerInfoReq req = new PlayerInfoReq();
                        req.Deserialize(buffer);
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
