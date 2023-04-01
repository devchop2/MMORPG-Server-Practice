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

        List<SkillInfo> skills = new List<SkillInfo>();

        public struct SkillInfo
        {
            public int skillId;
            public short level;
            public float duration;

            public bool Serialize(Span<byte> s, ref ushort count)
            {
                bool success = true;
                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), skillId);
                count += sizeof(int);
                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), level);
                count += sizeof(short);
                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), duration);
                count += sizeof(float);

                return success;
            }

            public void Deserialize(ReadOnlySpan<byte> s, ref ushort count)
            {

                skillId = BitConverter.ToInt32(s.Slice(count, s.Length - count));
                count += sizeof(int);
                level = BitConverter.ToInt16(s.Slice(count, s.Length - count));
                count += sizeof(short);
                duration = BitConverter.ToSingle(s.Slice(count, s.Length - count));
                count += sizeof(float);
            }
        }

        public PlayerInfoReq()
        {
            packetId = (ushort)PacketID.PlayerInfoReq;
            name = "";

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

            /* version 1
            ushort nameLen = (ushort)Encoding.Unicode.GetByteCount(name);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
            count += sizeof(ushort);

            Array.Copy(Encoding.Unicode.GetBytes(name), 0, segment.Array, count, nameLen);
            count += nameLen;
            */


            //version 2.
            ushort nameLen = (ushort)Encoding.Unicode.GetBytes(name, 0, name.Length, segment.Array, segment.Offset + count + sizeof(ushort)); //spacing size  Position
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);

            count += sizeof(ushort);
            count += nameLen;


            //send List
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)skills.Count);
            count += sizeof(ushort);

            foreach (var item in skills)
            {
                success &= item.Serialize(s, ref count);
            }
            //write size
            success &= BitConverter.TryWriteBytes(s, count);

            // 클라이언트가 그짓말할거라는거 생각해야 함 
            //패킷사이즈를 계산해서 마지막에 넣어줌. 대신, 첫번째 위치에 넣어줘야함.
            if (!success) return null;
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
            count += nameLen;
            //read list

            skills.Clear();
            ushort skillLen = (ushort)BitConverter.ToInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);

            Console.WriteLine("SkillLen:" + skillLen);

            for (int i = 0; i < skillLen; i++)
            {
                var skillInfo = new SkillInfo();
                skillInfo.Deserialize(s, ref count);
                skills.Add(skillInfo);
            }


            Console.WriteLine($"[PlayerName] {name}");
            foreach (var item in skills)
            {
                Console.WriteLine($"skill({item.skillId})({item.level})({item.duration}) :" + item.skillId);
            }
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
