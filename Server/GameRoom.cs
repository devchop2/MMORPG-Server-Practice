using System;
using ServerCore;

namespace Server
{
    public class GameRoom
    {
        public List<ClientSession> sessions = new List<ClientSession>();
        object lockObj = new object();

        public void Enter(ClientSession session)
        {
            lock (lockObj)
            {
                sessions.Add(session);
                session.gameRoom = this;
            }

        }
        public void Leave(ClientSession session)
        {
            lock (lockObj)
            {
                sessions.Remove(session);
            }

        }

        public void BroadCast(ClientSession session, string chat)
        {
            S_Chat broad = new S_Chat()
            {
                playerId = session.sessionId,
                chat = chat,
            };

            ArraySegment<byte> buffer = broad.Serialize();
            lock (lockObj)
            {
                foreach (var item in sessions)
                {
                    item.Send(buffer);
                }
            }

        }
    }
}

