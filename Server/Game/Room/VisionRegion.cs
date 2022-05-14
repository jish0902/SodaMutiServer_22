using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Game.Room
{
    public class VisionRegion
    {
        public Player Owner { get; private set; }

        public HashSet<GameObject> PreviousObjects { get; private set; } = new HashSet<GameObject>();

        public VisionRegion(Player owner)
        {
            Owner = owner;
        }


        public HashSet<GameObject> GatherObjects()
        {
            if (Owner == null || Owner.Room == null || Owner.CurrentRoomId == -1)
                return null;

            HashSet<GameObject> objects =  Owner.Room.Map.GetPlanetObjects(Owner.CurrentRoomId);

            if (objects == null)
                return null;

            return objects;
        }
        public HashSet<GameObject> GatherPlayers()
        {
            if (Owner == null || Owner.Room == null || Owner.CurrentRoomId == -1)
                return null;

            HashSet<GameObject> objects =  Owner.Room.Map.GetPlanetPlayers(Owner.CurrentRoomId).ToHashSet<GameObject>();

            if (objects == null)
                return null;
            return objects;
        }

        public void Update()
        {
            if (Owner == null || Owner.Room == null || Owner.CurrentRoomId == -1)
                return;

            HashSet<GameObject> currentObject = GatherObjects();
            currentObject.UnionWith(GatherPlayers());

            List<GameObject> added = currentObject.Except(PreviousObjects).ToList();
            if(added.Count > 0)
            {
                S_Spawn spawnPacket = new S_Spawn();
                foreach (GameObject go in added)
                {
                    if (go == Owner)
                        continue;

                    ObjectInfo info = new ObjectInfo();
                    info.MergeFrom(go.info);
                    spawnPacket.Objects.Add(info);//gameObject.info가 아니고 새로 만드는 이유 값이 계속 변경됨
                }
                Owner.Session.Send(spawnPacket);
            }

            List<GameObject> removed = PreviousObjects.Except(currentObject).ToList();
            if(removed.Count > 0)
            {
                S_Despawn despawnPacket = new S_Despawn();
                foreach (GameObject go in removed)
                {
                    if (go == Owner)
                        continue;
                    despawnPacket.ObjcetIds.Add(go.Id);
                }
                Owner.Session.Send(despawnPacket);
            }

            PreviousObjects = currentObject;

            Owner.Room.PushAfter(Program.ServerTick, Update); // .2초
        }//update

    }
}
