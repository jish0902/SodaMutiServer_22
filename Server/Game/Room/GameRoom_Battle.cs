using Google.Protobuf;
using Server.Game;
using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.Protocol;
using Server.Data;

namespace Server.Game
{
    public partial class GameRoom
    {
        public void HandleMove(Player player, C_Move packet)
        {
            if (player == null)
                return;

            //검사
            PositionInfo movePosInfo = packet.PositionInfo; //C요청
            
            player.info.PositionInfo.State = movePosInfo.State;
            player.info.PositionInfo.DirY = movePosInfo.DirY;
            player.info.PositionInfo.DirX = movePosInfo.DirX;
            player.info.PositionInfo.PosX = movePosInfo.PosX;
            player.info.PositionInfo.PosY = movePosInfo.PosY;
            player.info.PositionInfo.RotZ = movePosInfo.RotZ;
            
            
            //다른플레이어에게 알려줌
            S_Move resMovePacket = new S_Move();
            resMovePacket.ObjectId = player.info.ObjectId;
            resMovePacket.PositionInfo = packet.PositionInfo;

            BroadCast(player.CurrentRoomId, resMovePacket);
        }

        public void HandleSkill(Player player, C_Skill packet)
        {
            if (player == null)
                return;

            //검사
            Skill skill = null;
            if(DataManager.SkillDict.TryGetValue(packet.Info.SkillId, out skill) == false)
                return;

            // useSkill

            if (packet.Info.DirX != 0 || packet.Info.DirY != 0)
            {
                player.SkillDir = new System.Numerics.Vector2(packet.Info.DirX, packet.Info.DirY);
            }
            SkillManager.Instance.UseSkill(player, packet.Info.SkillId);

        }

        public void HandleHit(Player player, C_Hit packet)
        {
            int attackId = packet.AttackId;
            int hitId = packet.HitId;

            if(attackId == hitId || attackId == 0 || hitId == 0 || attackId == player.Id)
            { 
                Console.WriteLine("----------공격 오류-----------");
                return;
            }

            if(player.Id == hitId) //본인이 맞았는데 본인이 보고할경우
            {
                Projectile projectile = null;
                if(_projectilList.TryGetValue(hitId, out projectile) == true)
                {
                    player.OnDamaged(projectile.GetOwner(), projectile.Attack);
                }

                //player.Hp -= 

            }
            else //다른사람이 맞은걸 보고할경우
            {
               //Todo:
            }

            





        }
    }











}
   