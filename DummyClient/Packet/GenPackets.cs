
using ServerCore;
using System;
using System.Net;
using System.Text;

public enum PacketID
{
    C_Chat = 1,
S_Chat = 2,
}

public interface IPacket
{
    ushort Protocol { get; }
    void Deserialize(ArraySegment<byte> data);
    ArraySegment<byte> Serialize();
}



public class C_Chat : IPacket
{
    public string chat;
	

    public ushort Protocol => (ushort)PacketID.C_Chat;

    public ArraySegment<byte> Serialize()
    {

        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        ushort count = 0;
        bool success = true;

        count += sizeof(ushort); //size

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID. C_Chat); ;
        count += sizeof(ushort);
        
        ushort chatLen = (ushort)Encoding.Unicode.GetBytes(chat, 0, chat.Length, segment.Array, segment.Offset + count + sizeof(ushort)); 
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), chatLen);  
        count += sizeof(ushort);
        count += chatLen;

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
        
        ushort chatLen = (ushort)BitConverter.ToInt16(s.Slice(count, s.Length - count));
        count += sizeof(ushort);
        chat = Encoding.Unicode.GetString(s.Slice(count, chatLen));
        count += chatLen;

    }
}


public class S_Chat : IPacket
{
    public int playerId;
	public string chat;
	

    public ushort Protocol => (ushort)PacketID.S_Chat;

    public ArraySegment<byte> Serialize()
    {

        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        ushort count = 0;
        bool success = true;

        count += sizeof(ushort); //size

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID. S_Chat); ;
        count += sizeof(ushort);
        
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), playerId);
        count += sizeof(int);

        ushort chatLen = (ushort)Encoding.Unicode.GetBytes(chat, 0, chat.Length, segment.Array, segment.Offset + count + sizeof(ushort)); 
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), chatLen);  
        count += sizeof(ushort);
        count += chatLen;

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
        
        this.playerId = BitConverter.ToInt32(s.Slice(count, s.Length - count));  
        count += sizeof(int);

        ushort chatLen = (ushort)BitConverter.ToInt16(s.Slice(count, s.Length - count));
        count += sizeof(ushort);
        chat = Encoding.Unicode.GetString(s.Slice(count, chatLen));
        count += chatLen;

    }
}

