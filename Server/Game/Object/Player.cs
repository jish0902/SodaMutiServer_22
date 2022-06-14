using Google.Protobuf.Protocol;
using Server.Game;
using Server.Session;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;


namespace Server.Game
{
    public class Player : CreatureObj
    {
        public ClientSession Session { get; set; }
        public VisionRegion Vision { get; set; }
        public SkillCoolDown SkillCoolDown  = new SkillCoolDown();

        public Vector2 SkillDir { get; set; }
        public List<GameObject> Targets { get; set; } = new List<GameObject>();


        public Player()
        {
            ObjectType = GameObjectType.Player;
            Vision = new VisionRegion(this);

        }


        public bool ApplySkill(int id, float CoolDown)
        {
            int skillCool = SkillCoolDown.GetCoolTime(id);
            short currnt = (short)(DateTime.Now.Second + DateTime.Now.Minute * 60);
            //최소 : 0 최대 : 3660
            //Console.WriteLine(skillCool);
            //Console.WriteLine(currnt);
            float t = skillCool + CoolDown;
            if (currnt >= (t >= 3599 ? t - 3599 : t))
            {
                SkillCoolDown.SetCoolTime(id, currnt);
                return true;
            }
            return false;
        }




        public override void OnDamaged(GameObject attacker, int damage)
        {
            if (DamageReflexAction != null)
                DamageReflexAction(attacker);
            else 
                base.OnDamaged(attacker, damage);
        }

        public override void OnDead(GameObject attacker)
        {
            base.OnDead(attacker);
        }

       
    }
}
