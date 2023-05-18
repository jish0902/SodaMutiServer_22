using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Protocol;

namespace Server.Game;

public class VisionRegion
{
    public VisionRegion(Player owner)
    {
        Owner = owner;
    }

    public Player Owner { get; }

    public HashSet<GameObject> PreviousObjects { get; private set; } = new();


    public HashSet<GameObject> GatherObjects()
    {
        if (Owner == null || Owner.gameRoom == null || Owner.CurrentRoomId == -1)
            return null;

        var objects = Owner.gameRoom.Map.GetPlanetObjects(Owner.CurrentRoomId);

        if (objects == null)
            return null;

        return objects;
    }

    public HashSet<GameObject> GatherPlayers()
    {
        if (Owner == null || Owner.gameRoom == null || Owner.CurrentRoomId == -1)
            return null;

        var objects = Owner.gameRoom.Map.GetPlanetPlayers(Owner.CurrentRoomId).ToHashSet<GameObject>();

        if (objects == null)
            return null;
        return objects;
    }

    public void Update()
    {
        if (Owner == null || Owner.gameRoom == null || Owner.CurrentRoomId == -1)
            return;

        var currentObject = GatherObjects();
        currentObject.UnionWith(GatherPlayers());

        var added = currentObject.Except(PreviousObjects).ToList();
        if (added.Count > 0)
        {
            var spawnPacket = new S_Spawn();
            foreach (var go in added)
            {
                if (go == Owner)
                    continue;

                var info = new ObjectInfo();
                info.MergeFrom(go.info);
                spawnPacket.Objects.Add(info); //gameObject.info가 아니고 새로 만드는 이유 값이 계속 변경됨
            }

            Owner.Session.Send(spawnPacket);
        }

        var removed = PreviousObjects.Except(currentObject).ToList();
        if (removed.Count > 0)
        {
            var despawnPacket = new S_Despawn();
            foreach (var go in removed)
            {
                if (go == Owner)
                    continue;
                despawnPacket.ObjcetIds.Add(go.Id);
            }

            Owner.Session.Send(despawnPacket);
        }

        PreviousObjects = currentObject;

        Owner.gameRoom.PushAfter(Program.ServerIntervalTick, Update); // .2초
    } //update
}