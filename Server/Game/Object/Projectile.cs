using Google.Protobuf.Protocol;
using Server.Data;

namespace Server.Game;

public  class Projectile : SkillObj
{
    public Projectile()
    {
        ObjectType = GameObjectType.Projectile;
    }


    public override void Update()
    {
    }

    public override bool CheakData()
    {
        throw new System.NotImplementedException();
    }

    public override void Destroy()
    {
        throw new System.NotImplementedException();
    }
}