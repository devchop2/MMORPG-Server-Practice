using System;
using Server;
using ServerCore;

public class PacketHandler
{
    public static void C_LeaveRoomHandler(Session session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        if (clientSession == null) return;
        var room = clientSession.gameRoom;
        if (room != null) room.Push(() => room.Leave(clientSession));
    }

    public static void C_MoveHandler(Session session, IPacket packet)
    {
        C_Move move = packet as C_Move;
        ClientSession clientSession = session as ClientSession;
        if (clientSession == null) return;

        //Console.WriteLine($"({move.posX},{move.posY},{move.posZ})");
        var room = clientSession.gameRoom;
        if (room != null) room.Push(() => room.Move(clientSession, move));

    }
}