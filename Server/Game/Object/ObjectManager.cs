using System.Collections.Generic;
using Google.Protobuf.Protocol;

namespace Server.Game;

internal class ObjectManager
{
    //[unused(1)] [type(7)] [Id(24)]
    private int _counter = 1;
    private readonly object _lock = new();
    private readonly Dictionary<int, Player> _players = new();
    public static ObjectManager Instance { get; } = new();

    public T Add<T>() where T : GameObject, new()
    {
        var gameObjcet = new T();

        lock (_lock)
        {
            gameObjcet.Id = GenerateId(gameObjcet.ObjectType);
            if (gameObjcet.ObjectType == GameObjectType.Player) 
                _players.Add(gameObjcet.Id, gameObjcet as Player);
        }

        return gameObjcet;
    }

    private int GenerateId(GameObjectType type) //[unused(1)] [type(7)] [Id(24)]
    {
        lock (_lock)
        {
            return ((int)type << 24) | _counter++;
        }
    }

    public static GameObjectType GetObjectTypeById(int id)
    {
        var type = (id >> 24) & 0x7f;
        return (GameObjectType)type;
    }

    public bool Remove(int objectId)
    {
        var objectType = GetObjectTypeById(objectId);
        lock (_lock)
        {
            if (objectType == GameObjectType.Player)
                return _players.Remove(objectId);
        }

        return false;
    }

    public Player Find(int objectId)
    {
        var objectType = GetObjectTypeById(objectId);

        lock (_lock)
        {
            if (objectType == GameObjectType.Player)
            {
                Player player = null;
                if (_players.TryGetValue(objectId, out player))
                    return player;
            }
        }

        return null;
    }
}