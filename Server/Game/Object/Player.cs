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
        public List<GameObject> Targets { get; set; } = new List<GameObject>(); //스킬 공격
        
        //-------------------------------------------
        private List<Room> ownRooms = new List<Room>();

        public Player()
        {
            ObjectType = GameObjectType.Player;
            Vision = new VisionRegion(this);

        }

        #region InGames

        public void AddOwnRoomList(Room room)
        {
            ownRooms.Add(room);
        }

        public void RemoveRoomList(Room room)
        {
            ownRooms.Remove(room);
        }
        
        
        
        
        #endregion





        
        #region Skill
        
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
        

        public bool CheakSkill(int id, float CoolDown)  //Todo : 사용하기
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

        #endregion
       


        public override void OnDamaged(GameObject attacker, int damage)
        {
            if (DamageReflexAction != null)
                DamageReflexAction(attacker);
            else 
                base.OnDamaged(attacker, damage);
        }

        public override void OnDead(GameObject attacker)
        {
            if (gameRoom == null)
                return;

            S_Die diePacket = new S_Die();
            diePacket.ObjectId = Id;
            diePacket.AttackerId = attacker.Id;

            gameRoom.BroadCast(CurrentRoomId, diePacket);

            GameRoom room = gameRoom;
            room.Push(room.LeaveGame, Id);
        }

       
    }
}
