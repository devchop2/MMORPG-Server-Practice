using System;
namespace DummyClient
{
    public class SessionManager
    {
        static SessionManager _instance = new SessionManager();
        public static SessionManager Instance { get { return _instance; } }

        List<ServerSession> _sessions = new List<ServerSession>();
        object lockObj = new object();

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
                foreach (var item in _sessions)
                {
                    C_Chat chatting = new C_Chat()
                    {
                        chat = "Hello Server",
                    };
                    item.Send(chatting.Serialize());
                }
            }
        }

    }
}

