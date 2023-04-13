using System;
namespace DummyClient
{
    public class SessionManager
    {
        static SessionManager _instance = new SessionManager();
        public static SessionManager Instance { get { return _instance; } }

        List<ServerSession> _sessions = new List<ServerSession>();
        object lockObj = new object();
        Random _rand = new Random();

        public ServerSession Generate()
        {
            lock (lockObj)
            {
                ServerSession session = new ServerSession();
                _sessions.Add(session);
                return session;
            }
        }

        public void SendForEach()
        {
            lock (lockObj)
            {
                foreach (var session in _sessions)
                {
                    C_Move move = new C_Move()
                    {
                        posX = _rand.Next(-50, 50),
                        posY = 0,
                        posZ = _rand.Next(-50, 50),
                    };

                    session.Send(move.Serialize());
                }
            }
        }

    }
}

