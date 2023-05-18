using System;
using System.Collections.Generic;
using System.Threading;
using Collision.Shapes;
using Google.Protobuf.Protocol;
using Server.Data;
using Server.Game.Utils;
using ServerCore;

namespace Server.Game;

public class HitActor : IComparable<HitActor>
{
    public ushort nextHitTick;
    public GameObject hitObj;

    public HitActor(ushort _hitTick, GameObject _hitObj)
    {
        nextHitTick = _hitTick;
        hitObj = _hitObj;
    }
    public int CompareTo(HitActor other)
    {
        if (nextHitTick == other.nextHitTick)
            return 0;
        return nextHitTick < other.nextHitTick ? 1 : -1;
    }
}
public class ScopeSkill : SkillObj
{
    private PriorityQueue<HitActor> _pq = new PriorityQueue<HitActor>();
    
    public ScopeSkill(Shape shape)
    {
        active = true;
        ObjectType = GameObjectType.Scopeskill;
        _shape = shape;
    }


    public override bool CheakData()
    {
        if (Data == null ||  OwnerId == -1 || gameRoom == null || destroyed)
            return false;
        return true;
    }

    private List<GameObject> _hitList = new List<GameObject>();

    public override void OnCollisionFeedback(GameObject other)
    {
        
    }

    public override void OnCollision(GameObject other)
    {
        if (other != null || active || other.gameRoom != null || other.gameRoom == this.gameRoom ||
            other.State != CreatureState.Dead) 
        {
            if (_hitList.Contains(other) == false) // 적합하고 충돌에 없다면
            {
                _hitList.Add(other);
                other.OnDamaged(this.GetOwner() ,this.Attack);
                _pq.Push(new HitActor(
                    (ushort)(Program.ServerTickCount + Time.Millis2ServerTick((Data.cooldown * 1000))), other));
            }
        }
    }
    //약간 말이 안됨 초기화 되고 맞은애랑 되기 직전에 맞은애랑 초기화 되면 다시 같이 처맞음 
    public override void Update()
    {
        base.Update();
        
        CheakHit();
        _hitList.Clear();

    }
    
    public void CheakHit()
    {
        while (true)
        {
            var now = Program.ServerTickCount;

            if (_pq.Count == 0)
                return;

            var _hitActor = _pq.Peek();
            if (_hitActor.nextHitTick > now)
                break;

            _pq.Pop();

            GameObject target = _hitActor.hitObj;
            if (_hitList.Contains(target))
            {
                target.OnDamaged(this.GetOwner(),Data.damage);
            }
        }
    }
    
    
    
    public override void Destroy()
    {
        if (Data == null || OwnerId == -1 || gameRoom == null)
            Console.WriteLine("Destroy 오류");

        gameRoom.Push(() => { ObjectManager.Instance.Remove(Id); });
        gameRoom.Push(gameRoom.LeaveGame, Id);
        destroyed = true;
        Console.WriteLine("Destory");
    }
}