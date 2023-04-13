using System;
using System.Text;
using ServerCore;

public class PacketHandler
{
    public static void S_BroadcastEnterGameHandler(Session session, IPacket packet)
    {
        ServerSession s = session as ServerSession;
        S_BroadcastEnterGame enter = packet as S_BroadcastEnterGame;
    }

    public static void S_BroadcastLeaveRoomHandler(Session session, IPacket packet)
    {
        ServerSession s = session as ServerSession;
        S_BroadcastLeaveRoom enter = packet as S_BroadcastLeaveRoom;
    }

    public static void S_PlayerListHandler(Session session, IPacket packet)
    {
        ServerSession s = session as ServerSession;
        S_PlayerList enter = packet as S_PlayerList;
    }

    public static void S_BroadcastMoveHandler(Session session, IPacket packet)
    {
        ServerSession s = session as ServerSession;
        S_BroadcastMove enter = packet as S_BroadcastMove;
    }

}

