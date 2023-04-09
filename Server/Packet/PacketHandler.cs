using System;
using Server;
using ServerCore;

public class PacketHandler
{
    public static void C_ChatHandler(Session session, IPacket packet)
    {
        C_Chat chat = packet as C_Chat;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.gameRoom == null) return;

        var room = clientSession.gameRoom;
        if (room != null) room.Push(() => room.BroadCast(clientSession, chat.chat));

        //Console.WriteLine($"[From Client]{chat.chat}");
    }
}