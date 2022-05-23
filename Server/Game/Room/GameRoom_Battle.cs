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

            //검사--------------------

            #region 방이동
            int next = packet.PositionInfo.CurrentRoomId;
            int now = player.CurrentRoomId;
            if (next != now)
            {
                Room.Room nowRoom =  Map.Rooms.Find(r => r.Id == now);
                if (nowRoom == null)
                {
                    Console.WriteLine("방 오류");
                    return;
                }
                Room.Room nextRoom = nowRoom.TouarableRooms.Find(t=> t.Id == next);
                if (nextRoom == null)
                {
                    Console.WriteLine($"이동 오류{nowRoom.TouarableRooms}");
                    return;
                }

                nowRoom.Players.Remove(player);
                nextRoom.Players.Add(player);
                player.CurrentRoomId = next;

                Console.WriteLine($"{player.info.Name}이 {now}에서 {next}로 이동");
                Console.WriteLine($"{nextRoom.Objects.Count}");
                foreach (GameObject go in nextRoom.Objects)
                {
                    Console.WriteLine($"{go.CellPos}{go.CurrentRoomId}{go.State}");
                }
            }
            #endregion


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

            if(packet.TargetIds.Count > 0)
            {
                List<GameObject> targets = new List<GameObject>();
                foreach (int target in packet.TargetIds)
                {
                    Player p;
                    if (_playerList.TryGetValue(target, out p) == true)
                        targets.Add(p);

                    Monster m;
                    if (_MonsterList.TryGetValue(target, out m) == true)
                        targets.Add(m);
                }
                player.Targets = targets;
            }

            SkillManager.Instance.UseSkill(player, packet.Info.SkillId);

        }

        public void HandleHit(Player player, C_Hit packet)
        {
            int attackId = packet.AttackId; //화살아이디
            int hitId = packet.HitId;  //맞은아이디

            if(attackId == hitId || attackId == 0 || hitId == 0 || attackId == player.Id  )
            { 
                Console.WriteLine("----------공격 오류-----------");
                return;
            }

            if(player.Id == hitId) //본인이 맞았는데 본인이 보고할경우
            {
                Projectile projectile = null;
                if(_projectilList.TryGetValue(attackId, out projectile) == true)
                {
                    if (projectile.GetOwner() == player)
                    {
                        return;  //본인이 본인을 때릴경우
                    }

                    player.OnDamaged(projectile.GetOwner(), projectile.Attack);
                    Console.WriteLine($"{projectile.GetOwner().info.Name}이 {player.info.Name}를 {projectile.Attack}만큼 공격");

                    this.Push(LeaveGame,attackId);
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
   