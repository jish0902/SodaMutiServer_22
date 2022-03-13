using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    class ObjectManager
    {
        public static ObjectManager Instance { get; } = new ObjectManager();
        object _lock = new object();
        Dictionary<int, Player> _players = new Dictionary<int, Player>();

        //[unused(1)] [type(7)] [Id(24)]
        int _counter = 1;

        public T Add<T>() where T : GameObject, new()
        {
            T gameObjcet = new T();

            lock (_lock)
            {
                gameObjcet.Id = GenerateId(gameObjcet.ObjectType);
                if(gameObjcet.ObjectType == GameObjectType.Player)
                {
                    _players.Add(gameObjcet.Id, gameObjcet as Player);
                }
            }

            return gameObjcet;
        }

        int GenerateId (GameObjectType type)
        {

            lock (_lock)
            {
                return ((int)type << 24 | (_counter++));
            }

        }

        public static GameObjectType GetObjectTypeById(int id)
        {
            int type = (id >> 24) & 0x7f;
            return (GameObjectType)type;
        }

        public bool Remove(int objectId)
        {
            GameObjectType objectType = GetObjectTypeById(objectId);
            lock (_lock)
            {
                if (objectType == GameObjectType.Player)
                    return _players.Remove(objectId);
            }
            return false;
        }

        public Player Find(int objectId)
        {
            GameObjectType objectType = GetObjectTypeById(objectId);

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
}
