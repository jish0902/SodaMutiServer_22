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
            ObjectInfo info = player.info;
            

            info.PositionInfo.State = movePosInfo.State;
            info.PositionInfo.DirY = movePosInfo.DirY;
            info.PositionInfo.DirX = movePosInfo.DirX;
            info.PositionInfo.PosX = movePosInfo.PosX;
            info.PositionInfo.PosY = movePosInfo.PosY;
            
            //다른플레이어에게 알려줌
            S_Move resMovePacket = new S_Move();
            resMovePacket.ObjectId = player.info.ObjectId;
            resMovePacket.PositionInfo = packet.PositionInfo;

            BroadCast(player.CurrentPlanetId, resMovePacket);
        }

        public void HandleSkill(Player player, C_Skill packet)
        {
            if (player == null)
                return;

            //검사

            S_Skill skillPacket = new S_Skill() { Info = new SkillInfo() };
            skillPacket.ObjectId = player.info.ObjectId;
            skillPacket.Info.SkillId = packet.Info.SkillId;

            BroadCast(player.CurrentPlanetId,skillPacket);


            Skill skill = null;
            if(DataManager.SkillDict.TryGetValue(skillPacket.Info.SkillId, out skill) == false)
                return;

            if(skill.id == 101)
            {

            }
        }
    }
}
   