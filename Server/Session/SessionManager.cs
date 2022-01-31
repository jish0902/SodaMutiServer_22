using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Session
{
    class SessionManager
    {
        static SessionManager _instance = new SessionManager();
        public static SessionManager Instance { get { return _instance; } }

        int _sessionId = 0;
        Dictionary<int, ClientSession> _sessions = new Dictionary<int, ClientSession>();
        object _lock = new object();

        public int GetBusySocre()
        {
            int count = 0;
            lock (_lock)
            {
                count = _sessions.Count;
            }
            return count;
        }

        public List<ClientSession> GetSessions()
        {
            List<ClientSession> sessions = new List<ClientSession>();

            lock (_lock)
            {
                sessions = _sessions.Values.ToList();
            }

            return sessions;
        }


        public ClientSession Generate()
        {
            ClientSession session = new ClientSession();
            lock (_lock)
            {
                session.SessionId = _sessionId++;
                _sessions.Add(_sessionId, session);
            }
            Console.WriteLine($"Connected ({_sessions.Count}) Player");

            return session;
        }

        public ClientSession Find(int id)
        {
            ClientSession session = null;
            lock (_lock)
            {
                _sessions.TryGetValue(id, out session);
            }
            return session;
        }
        public void Remove(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Remove(session.SessionId);
            }
            Console.WriteLine($"Conntected ({_sessions.Count}) Players");
        }
    }
}
