using System;
using System.Text;
using ServerCore;

public class PacketHandler
{
    public static void S_ChatHandler(Session sesion, IPacket packet)
    {
        S_Chat chatting = packet as S_Chat;

        Console.WriteLine($"[{chatting.playerId}] {chatting.chat}\n");

    }
}

