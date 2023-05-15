using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Session;

internal class SessionManager
{
    private readonly object _lock = new();

    private int _sessionId;
    private readonly Dictionary<int, ClientSession> _sessions = new();
    public static SessionManager Instance { get; } = new();

    public int GetBusySocre()
    {
        var count = 0;
        lock (_lock)
        {
            count = _sessions.Count;
        }

        return count;
    }

    public List<ClientSession> GetSessions()
    {
        var sessions = new List<ClientSession>();

        lock (_lock)
        {
            sessions = _sessions.Values.ToList();
        }

        return sessions;
    }


    public ClientSession Generate()
    {
        var session = new ClientSession();
        lock (_lock)
        {
            session.SessionId = _sessionId++;
            _sessions.Add(_sessionId, session); // session.SessionId 아닌가?
        }

        Console.WriteLine($"Connected ({_sessions.Count}) Player");

        return session;
    }

    /*
     a += 3

     a => 0
     0 + 1
     a <= 0
     
     a => 0
     0 -1
     a= -1
     
    
     */

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

        Console.WriteLine($"DisConnected ({_sessions.Count}) Players");
    }
}