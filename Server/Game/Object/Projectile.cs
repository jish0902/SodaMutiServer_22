using Google.Protobuf.Protocol;
using Server.Data;

namespace Server.Game;

public class Projectile : GameObject
{
    public Projectile()
    {
        ObjectType = GameObjectType.Projectile;
    }

    public Skill Data { get; set; }

    public override void Update()
    {
    }
}