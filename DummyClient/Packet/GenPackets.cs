
using ServerCore;
using System;
using System.Net;
using System.Text;

public enum PacketID
{
    S_BroadcastEnterGame = 1,
C_LeaveRoom = 2,
S_BroadcastLeaveRoom = 3,
S_PlayerList = 4,
C_Move = 5,
S_BroadcastMove = 6,
}

public interface IPacket
{
    ushort Protocol { get; }
    void Deserialize(ArraySegment<byte> data);
    ArraySegment<byte> Serialize();
}



public class S_BroadcastEnterGame : IPacket
{
    public int playerId;
	public float posX;
	public float posY;
	public float posZ;
	

    public ushort Protocol => (ushort)PacketID.S_BroadcastEnterGame;

    public ArraySegment<byte> Serialize()
    {

        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        ushort count = 0;
        bool success = true;

        count += sizeof(ushort); //size

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID. S_BroadcastEnterGame); ;
        count += sizeof(ushort);
        
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), playerId);
        count += sizeof(int);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), posX);
        count += sizeof(float);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), posY);
        count += sizeof(float);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), posZ);
        count += sizeof(float);

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

        this.posX = BitConverter.ToSingle(s.Slice(count, s.Length - count));  
        count += sizeof(float);

        this.posY = BitConverter.ToSingle(s.Slice(count, s.Length - count));  
        count += sizeof(float);

        this.posZ = BitConverter.ToSingle(s.Slice(count, s.Length - count));  
        count += sizeof(float);

    }
}


public class C_LeaveRoom : IPacket
{
    

    public ushort Protocol => (ushort)PacketID.C_LeaveRoom;

    public ArraySegment<byte> Serialize()
    {

        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        ushort count = 0;
        bool success = true;

        count += sizeof(ushort); //size

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID. C_LeaveRoom); ;
        count += sizeof(ushort);
        
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
        
    }
}


public class S_BroadcastLeaveRoom : IPacket
{
    public int playerId;
	

    public ushort Protocol => (ushort)PacketID.S_BroadcastLeaveRoom;

    public ArraySegment<byte> Serialize()
    {

        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        ushort count = 0;
        bool success = true;

        count += sizeof(ushort); //size

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID. S_BroadcastLeaveRoom); ;
        count += sizeof(ushort);
        
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), playerId);
        count += sizeof(int);

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

    }
}


public class S_PlayerList : IPacket
{
    public List<Player> players = new List<Player>();
    public struct Player
    {
        public bool isSelf;
	public int playerId;
	public float posX;
	public float posY;
	public float posZ;
	
            
        public bool Serialize(Span<byte> s, ref ushort count)
        {
            bool success = true;
            
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), isSelf);
        count += sizeof(bool);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), playerId);
        count += sizeof(int);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), posX);
        count += sizeof(float);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), posY);
        count += sizeof(float);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), posZ);
        count += sizeof(float);

            return success;
        }

        public void Deserialize(ReadOnlySpan<byte> s, ref ushort count)
        {
            
        this.isSelf = BitConverter.ToBoolean(s.Slice(count, s.Length - count));  
        count += sizeof(bool);

        this.playerId = BitConverter.ToInt32(s.Slice(count, s.Length - count));  
        count += sizeof(int);

        this.posX = BitConverter.ToSingle(s.Slice(count, s.Length - count));  
        count += sizeof(float);

        this.posY = BitConverter.ToSingle(s.Slice(count, s.Length - count));  
        count += sizeof(float);

        this.posZ = BitConverter.ToSingle(s.Slice(count, s.Length - count));  
        count += sizeof(float);

        }
    }

	

    public ushort Protocol => (ushort)PacketID.S_PlayerList;

    public ArraySegment<byte> Serialize()
    {

        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        ushort count = 0;
        bool success = true;

        count += sizeof(ushort); //size

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID. S_PlayerList); ;
        count += sizeof(ushort);
        
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)this.players.Count);
        count += sizeof(ushort);
        foreach (var item in this.players) success &= item.Serialize(s, ref count);

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
        
        this.players.Clear();
        ushort playerLen = (ushort)BitConverter.ToInt16(s.Slice(count, s.Length - count));
        count += sizeof(ushort);

        for (int i = 0; i < playerLen; i++)
        {
            Player playerInfo = new Player();
            playerInfo.Deserialize(s, ref count);
            this.players.Add(playerInfo);
        }

    }
}


public class C_Move : IPacket
{
    public float posX;
	public float posY;
	public float posZ;
	

    public ushort Protocol => (ushort)PacketID.C_Move;

    public ArraySegment<byte> Serialize()
    {

        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        ushort count = 0;
        bool success = true;

        count += sizeof(ushort); //size

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID. C_Move); ;
        count += sizeof(ushort);
        
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), posX);
        count += sizeof(float);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), posY);
        count += sizeof(float);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), posZ);
        count += sizeof(float);

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
        
        this.posX = BitConverter.ToSingle(s.Slice(count, s.Length - count));  
        count += sizeof(float);

        this.posY = BitConverter.ToSingle(s.Slice(count, s.Length - count));  
        count += sizeof(float);

        this.posZ = BitConverter.ToSingle(s.Slice(count, s.Length - count));  
        count += sizeof(float);

    }
}


public class S_BroadcastMove : IPacket
{
    public int playerId;
	public float posX;
	public float posY;
	public float posZ;
	

    public ushort Protocol => (ushort)PacketID.S_BroadcastMove;

    public ArraySegment<byte> Serialize()
    {

        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        ushort count = 0;
        bool success = true;

        count += sizeof(ushort); //size

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID. S_BroadcastMove); ;
        count += sizeof(ushort);
        
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), playerId);
        count += sizeof(int);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), posX);
        count += sizeof(float);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), posY);
        count += sizeof(float);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), posZ);
        count += sizeof(float);

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

        this.posX = BitConverter.ToSingle(s.Slice(count, s.Length - count));  
        count += sizeof(float);

        this.posY = BitConverter.ToSingle(s.Slice(count, s.Length - count));  
        count += sizeof(float);

        this.posZ = BitConverter.ToSingle(s.Slice(count, s.Length - count));  
        count += sizeof(float);

    }
}

