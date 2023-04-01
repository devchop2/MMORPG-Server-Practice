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
        public string name; //가변길

        public List<SkillInfo> skills = new List<SkillInfo>();

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
            Console.WriteLine((ushort)skills.Count);
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
            Console.WriteLine($"[PlayerName] {name}");

            //read list

            skills.Clear();
            ushort skillLen = (ushort)BitConverter.ToInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);

            for (int i = 0; i < skillLen; i++)
            {
                var skillInfo = new SkillInfo();
                skillInfo.Deserialize(s, ref count);
                skills.Add(skillInfo);
            }


        }
    }

    public class ServerSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine("On Connected :" + endPoint);

            PlayerInfoReq testPacket = new PlayerInfoReq() { playerId = 1001, name = "devchop2" };
            testPacket.skills.Add(new PlayerInfoReq.SkillInfo() { skillId = 1, level = 1, duration = 0.5f });
            testPacket.skills.Add(new PlayerInfoReq.SkillInfo() { skillId = 2, level = 10, duration = 3.5f });

            var sendBuff = testPacket.Serialize();

            if (sendBuff != null) Send(sendBuff);
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
