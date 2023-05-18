using System;
using Google.Protobuf.Protocol;

namespace Server.Game;

public class CreatureObj : GameObject
{
    public Action<GameObject> DamageReflexAction;

    public override void OnCollision(GameObject other)
    {
        if (other.ObjectType == GameObjectType.Projectile)
        {
            OnDamaged(other,other.Attack);
            other.OnCollisionFeedback(this);
        }
        else if (other.ObjectType == GameObjectType.Scopeskill)
        {
            //할일 TODO : 장판 데이미지 일정량만 받게(일시 무적)
            OnDamaged(other,other.Attack);
            other.OnCollisionFeedback(this);
        }
        
        
        
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="attacker">스킬 or 장판 or 발사체</param>
    /// <param name="damage"></param>
    public override void OnDamaged(GameObject attacker, int damage) //할일 TODO : 데미지 말고 영향(class damaage effect)등 고려
    {
        if (gameRoom == null)
            return;

        GameObject rootAttacker = attacker;

        if (attacker.ObjectType == GameObjectType.Monster || attacker.ObjectType == GameObjectType.Player)
            rootAttacker = attacker.GetOwner();
        //할일 TODO : 만약 주인이 또 스킬이거나 하는 경우!!
        
        damage = Math.Max(damage, 0);
        stat.Hp = Math.Max(stat.Hp - damage, 0);

        Console.WriteLine($" attacker :{rootAttacker.Id} Damage : {damage}  stat.Hp : {stat.Hp}");

        var ChangePacket = new S_ChangeHp();
        ChangePacket.ObjectId = Id;
        ChangePacket.Hp = stat.Hp;
        gameRoom.BroadCast(CurrentRoomId, ChangePacket);

        if (stat.Hp <= 0)
        {
            stat.Hp = 0;
            OnDead(rootAttacker);
        }
    }
}