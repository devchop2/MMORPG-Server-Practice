
using ServerCore;
using System;
using System.Net;
using System.Text;

public enum PacketID
{
    PlayerInfoReq = 1,
}



public class PlayerInfoReq
{
    public long playerId;
    public string name;
    public List<Skill> skills = new List<Skill>();
    public struct Skill
    {
        public int id;
        public short level;
        public float duration;


        public bool Serialize(Span<byte> s, ref ushort count)
        {
            bool success = true;

            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), id);
            count += sizeof(int);

            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), level);
            count += sizeof(short);

            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), duration);
            count += sizeof(float);

            return success;
        }

        public void Deserialize(ReadOnlySpan<byte> s, ref ushort count)
        {

            this.id = BitConverter.ToInt32(s.Slice(count, s.Length - count));
            count += sizeof(int);

            this.level = BitConverter.ToInt16(s.Slice(count, s.Length - count));
            count += sizeof(short);

            this.duration = BitConverter.ToSingle(s.Slice(count, s.Length - count));
            count += sizeof(float);

        }
    }


    public ArraySegment<byte> Serialize()
    {

        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        ushort count = 0;
        bool success = true;

        count += sizeof(ushort); //size

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.PlayerInfoReq); ;
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), playerId);
        count += sizeof(long);

        ushort nameLen = (ushort)Encoding.Unicode.GetBytes(name, 0, name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
        count += sizeof(ushort);
        count += nameLen;

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)this.skills.Count);
        count += sizeof(ushort);
        foreach (var item in this.skills) success &= item.Serialize(s, ref count);

        success &= BitConverter.TryWriteBytes(s, count);

        if (!success) return null;
        return SendBufferHelper.Close(count);

    }

    public void Deserialize(ArraySegment<byte> data)
    {

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(data.Array, data.Offset, data.Count);
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);

        this.playerId = BitConverter.ToInt64(s.Slice(count, s.Length - count));
        count += sizeof(long);

        ushort nameLen = (ushort)BitConverter.ToInt16(s.Slice(count, s.Length - count));
        count += sizeof(ushort);
        name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
        count += nameLen;

        this.skills.Clear();
        ushort skillLen = (ushort)BitConverter.ToInt16(s.Slice(count, s.Length - count));
        count += sizeof(ushort);

        for (int i = 0; i < skillLen; i++)
        {
            Skill skillInfo = new Skill();
            skillInfo.Deserialize(s, ref count);
            this.skills.Add(skillInfo);
        }

    }
}

