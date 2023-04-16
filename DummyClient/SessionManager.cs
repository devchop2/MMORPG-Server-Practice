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
                        posX = _rand.Next(-10, 10),
                        posY = _rand.Next(-7, 7),
                        posZ = 0,
                    };

                    session.Send(move.Serialize());
                }
            }
        }

    }
}

