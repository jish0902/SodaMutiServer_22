using Google.Protobuf.Protocol;
using Server.Data;

namespace Server.Game;

public class ScopeSkill : GameObject
{
    public ScopeSkill()
    {
        ObjectType = GameObjectType.Scopeskill;
    }
    
    public Skill Data { get; set; }


    public override void OnCollisionFeedback(GameObject other = null)
    {
        
    }

    public override void Update()
    {
    }
}