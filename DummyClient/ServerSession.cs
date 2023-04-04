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

            C_PlayerInfoReq testPacket = new C_PlayerInfoReq() { playerId = 1001, name = "devchop2" };
            testPacket.skills.Add(new C_PlayerInfoReq.Skill() { id = 1, level = 1, duration = 0.5f });
            testPacket.skills.Add(new C_PlayerInfoReq.Skill() { id = 2, level = 10, duration = 3.5f });

            var sendBuff = testPacket.Serialize();

            if (sendBuff != null) Send(sendBuff);
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine("OnDisconnected :" + endPoint);
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Server] {recvData}\n");
            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine("Send Complete. total Bytes:" + numOfBytes);
        }
    }
}
