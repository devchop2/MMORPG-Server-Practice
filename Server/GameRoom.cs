using System;
using ServerCore;

namespace Server
{
    public class GameRoom : IJobQueue
    {
        public List<ClientSession> sessions = new List<ClientSession>();


        public void Enter(ClientSession session)
        {
            sessions.Add(session);
            session.gameRoom = this;
        }
        public void Leave(ClientSession session)
        {
            sessions.Remove(session);
        }

        public void BroadCast(ClientSession session, string chat)
        {
            S_Chat broad = new S_Chat()
            {
                playerId = session.sessionId,
                chat = chat,
            };

            ArraySegment<byte> buffer = broad.Serialize();
            foreach (var item in sessions)
            {
                item.Send(buffer);
            }
        }

        #region Job Queue

        JobQueue jobQueue = new JobQueue();
        public void Push(Action action)
        {
            jobQueue.Push(action);
        }

        public Action Pop()
        {
            return jobQueue.Pop()
        }

        #endregion
    }
}

