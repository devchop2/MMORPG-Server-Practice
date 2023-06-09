
using System;
using ServerCore;


public class PacketManager
{

    Dictionary<ushort, Action<Session, ArraySegment<byte>>> recvHandlers = new Dictionary<ushort, Action<Session, ArraySegment<byte>>>();
    Dictionary<ushort, Action<Session, IPacket>> handler = new Dictionary<ushort, Action<Session, IPacket>>();

    public void Register()
    {
      recvHandlers.Add((ushort)PacketID.PlayerInfoReq, MakePacket<PlayerInfoReq>);
        handler.Add((ushort)PacketID.PlayerInfoReq, PacketHandler.PlayerInfoReqHandler);

    }

    static PacketManager _instance = null;
    public static PacketManager Instance
    {
        get
        {
            if (_instance == null) _instance = new PacketManager();
            return _instance;
        }
    }

    public void OnRecvPacket(Session session, ArraySegment<byte> buffer)
    {
        string recvData = BitConverter.ToString(buffer.Array, buffer.Offset, buffer.Count);

        
        ushort count = 0;
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += sizeof(ushort);
        ushort packetId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += sizeof(ushort);

        if (recvHandlers.TryGetValue(packetId, out var act))
        {
            act.Invoke(session, buffer);
        }
    }

    void MakePacket<T>(Session session, ArraySegment<byte> buffer) where T : IPacket, new()
    {
        T p = new T();
        p.Deserialize(buffer);

        if (handler.TryGetValue(p.Protocol, out var act))
        {
            act?.Invoke(session, p);
        }

    }
}
