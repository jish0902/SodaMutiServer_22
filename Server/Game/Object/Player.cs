using Google.Protobuf.Protocol;
using Server.Game.Room;
using Server.Session;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class Player : GameObject
    {
        public ClientSession Session { get; set; }
        public VisionRegion Vision { get; set; }

        public Player()
        {
            ObjectType = GameObjectType.Player;
            Vision = new VisionRegion(this);

        }
        public override void OnDamaged(GameObject attacker, int damage)
        {
            base.OnDamaged(attacker, damage);
        }

        public override void OnDead(GameObject attacker)
        {
            base.OnDead(attacker);
        }

       
    }
}
