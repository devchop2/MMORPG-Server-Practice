using System.Net;
using System.Text;
using ServerCore;

namespace Server
{
    public class ClientSession : Session
    {
        public int sessionId;
        public GameRoom gameRoom { get; set; }

        public override void OnConnected(EndPoint endPoint)
        {
            var room = Program.room;
            room.Push(() => room.Enter(this));
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            SessionManager.Instance.Remove(this);

            var room = gameRoom;
            if (room != null) room.Push(() => room.Leave(this));

            gameRoom = null;
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
