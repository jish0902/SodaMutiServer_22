using System.Collections.Generic;

namespace Server.Game;

internal class GameLogic : JobSerializer
{
    private int _roomId = 1;
    private readonly Dictionary<int, GameRoom> _rooms = new();
    public static GameLogic Instance { get; } = new();

    public void Update()
    {
        Flush();

        foreach (var room in _rooms.Values) 
            room.Update();
    }

    public GameRoom Add(int mapId)
    {
        var gameRoom = new GameRoom();
        gameRoom.Push(gameRoom.Init, mapId, 9);

        gameRoom.RoomId = _roomId;
        _rooms.Add(_roomId, gameRoom);
        _roomId++;
        return gameRoom;
    }

    public bool Remove(int roomId)
    {
        return _rooms.Remove(roomId);
    }

    public GameRoom Find(int roomId)
    {
        GameRoom room = null;
        if (_rooms.TryGetValue(roomId, out room))
            return room;
        return null;
    }
}