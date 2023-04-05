using ServerCore;
using System;
using System.Net;
using System.Text;

namespace DummyClient
{

    public class ServerSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine("On Connected :" + endPoint);
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine("OnDisconnected :" + endPoint);
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine("Send Complete. total Bytes:" + numOfBytes);
        }
    }
}
