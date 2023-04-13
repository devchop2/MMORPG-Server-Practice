using ServerCore;
using System;
using System.Net;
using System.Text;

public class ServerSession : PacketSession
{
    public override void OnConnected(EndPoint endPoint)
    {
        Console.WriteLine("On Connected :" + endPoint);
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        Console.WriteLine("OnDisconnected :" + endPoint);
    }

    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        PacketManager.Instance.OnRecvPacket(this, buffer);
    }

    public override void OnSend(int numOfBytes)
    {
        //Console.WriteLine("Send Complete. total Bytes:" + numOfBytes);
    }
}