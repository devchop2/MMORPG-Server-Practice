
using System;
using ServerCore;
using System.Collections.Generic;


public class PacketManager
{

    static PacketManager _instance = new PacketManager();
    public static PacketManager Instance { get { return _instance; } }

    Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> makeFunc = new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>();
    Dictionary<ushort, Action<PacketSession, IPacket>> handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();

    public PacketManager(){ Register(); }
   
    public void Register()
    {

        makeFunc.Add((ushort)PacketID.C_LeaveRoom, MakePacket<C_LeaveRoom>);
        handler.Add((ushort)PacketID.C_LeaveRoom, PacketHandler.C_LeaveRoomHandler);

        makeFunc.Add((ushort)PacketID.C_Move, MakePacket<C_Move>);
        handler.Add((ushort)PacketID.C_Move, PacketHandler.C_MoveHandler);

    }

    

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession,IPacket> handler = null)
    {
        string recvData = BitConverter.ToString(buffer.Array, buffer.Offset, buffer.Count);

        
        ushort count = 0;
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += sizeof(ushort);
        ushort packetId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += sizeof(ushort);

        if (makeFunc.TryGetValue(packetId, out var act))
        {
            IPacket packet =  act.Invoke(session, buffer);
            if(handler != null) handler.Invoke(session, packet);
            else HandlePacket(session, packet);
        }
    }

    T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {
        T p = new T();
        p.Deserialize(buffer);
        return p;
    }

    public void HandlePacket(PacketSession session, IPacket p)
    {
        if (handler.TryGetValue(p.Protocol, out var act))
        {
            act?.Invoke(session, p);
        }
    }
}
    
