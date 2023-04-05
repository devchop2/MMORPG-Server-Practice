using System;
namespace Server
{
    public class SessionManager
    {
        static SessionManager _instance = new SessionManager();
        public static SessionManager Instance { get { return _instance; } }

        int sessionId = 0;
        Dictionary<int, ClientSession> _sessions = new Dictionary<int, ClientSession>();

        object lockObj = new object();

        public ClientSession Generate()
        {
            lock (lockObj)
            {
                int id = ++sessionId;
                ClientSession sess = new ClientSession();
                sess.sessionId = id;
                _sessions.Add(id, sess);
                return sess;
            }
        }

        public ClientSession Find(int id)
        {
            lock (lockObj)
            {
                _sessions.TryGetValue(id, out var session);
                return session;
            }

        }

        public void Remove(ClientSession session)
        {
            lock (lockObj)
            {
                _sessions.Remove(session.sessionId);
            }
        }
    }
}

