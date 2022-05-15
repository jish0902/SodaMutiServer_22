using Google.Protobuf.Protocol;
using Server.Game.Room;
using Server.Session;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;


namespace Server.Game
{
    public class Player : GameObject
    {
        public ClientSession Session { get; set; }
        public VisionRegion Vision { get; set; }
        public SkillCoolDown SkillCoolDown  = new SkillCoolDown();

        public Vector2 SkillDir { get; set; }
        public List<GameObject> Targets { get; set; }


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
            if (currnt >= (t >= 3660 ? t - 3660 : t))
            {
                SkillCoolDown.SetCoolTime(id, currnt);
                return true;
            }
            return false;
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
