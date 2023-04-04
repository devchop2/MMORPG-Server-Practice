using System;
using Server;
using ServerCore;

public class PacketHandler
{

    public static void C_PlayerInfoReqHandler(Session session, IPacket packet)
    {
        C_PlayerInfoReq p = packet as C_PlayerInfoReq;
        Console.WriteLine($"PlayerInfoReq:{p.name},{p.playerId}");

        foreach (var item in p.skills)
        {
            Console.WriteLine($"Skill({item.id},{item.level},{item.duration})");
        }
    }
}