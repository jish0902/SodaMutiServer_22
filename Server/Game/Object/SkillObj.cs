using Collision.Shapes;
using Google.Protobuf.Protocol;
using Server.Data;
using ServerCore;

namespace Server.Game;

public abstract class SkillObj : GameObject
{
    protected bool destroyed = false;
    protected bool active = true;
    public Shape  _shape;

    
    public SkillObj()
    {
        ObjectType = GameObjectType.None;
    }

    public Skill Data { get; set; }

    public override void Update()
    {
        if(CheakData() == false) //실패 : 데이터가 없어지거나 삭제되거나 
            return;
        
        gameRoom.PushAfter(Program.ServerIntervalTick, Update);
    }

    public abstract bool CheakData();

    public abstract void Destroy();

}