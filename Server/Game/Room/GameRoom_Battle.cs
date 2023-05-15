using System;
using System.Collections.Generic;
using System.Numerics;
using Google.Protobuf.Protocol;
using Server.Data;

namespace Server.Game;

public partial class GameRoom
{
    public void HandleMove(Player player, C_Move packet)
    {
        if (player == null)
            return;

        //검사--------------------

        #region 방이동

        var next = packet.PositionInfo.CurrentRoomId;
        var now = player.CurrentRoomId;
        if (next != now) //방이 다르면
        {
            Map.MoveRoom(player, next);

            //------------ 방 정보 ----------------------------------------
            var roomPacket = new S_RoomInfo();
            var nextRooms = new List<Room>();

            var room = Map.GetRoom(next);
            nextRooms.Add(room);
            nextRooms.AddRange(room.TouarableRooms);

            if (nextRooms != null)
                foreach (var r in nextRooms)
                {
                    var roomInfo = new RoomInfo();
                    roomInfo.RoomId = r.Id;
                    roomInfo.RoomLevel = r.RoomLevel;
                    roomInfo.RoomType = (int)r.RoomType;
                    roomInfo.PosX = r.PosX;
                    roomInfo.PosY = r.PosY;
                    roomPacket.RoomInfos.Add(roomInfo);
                }
            else
                Console.WriteLine("방이동 오류");
        }

        #endregion


        var movePosInfo = packet.PositionInfo; //C요청

        player.info.PositionInfo.State = movePosInfo.State;
        player.info.PositionInfo.DirY = movePosInfo.DirY;
        player.info.PositionInfo.DirX = movePosInfo.DirX;
        player.info.PositionInfo.PosX = movePosInfo.PosX;
        player.info.PositionInfo.PosY = movePosInfo.PosY;
        player.info.PositionInfo.RotZ = movePosInfo.RotZ;


        //다른플레이어에게 알려줌
        var resMovePacket = new S_Move();
        resMovePacket.ObjectId = player.info.ObjectId;
        resMovePacket.PositionInfo = packet.PositionInfo;


        Map.ApplyMove(player,
            new Vector2Int((int)Math.Round(packet.PositionInfo.PosX), (int)Math.Round(packet.PositionInfo.PosY)));

        BroadCast(player.CurrentRoomId, resMovePacket);
    }

    public void HandleSkill(Player player, C_Skill packet)
    {
        if (player == null)
            return;

        //검사
        Skill skill = null;
        if (DataManager.SkillDict.TryGetValue(packet.Info.SkillId, out skill) == false)
            return;

        // useSkill

        if (packet.Info.DirX != 0 || packet.Info.DirY != 0)
            player.SkillDir = new Vector2(packet.Info.DirX, packet.Info.DirY);

        if (packet.TargetIds.Count > 0)
        {
            var targets = new List<GameObject>();
            foreach (var target in packet.TargetIds)
            {
                Player p;
                if (_playerList.TryGetValue(target, out p))
                    targets.Add(p);

                Monster m;
                if (_MonsterList.TryGetValue(target, out m))
                    targets.Add(m);
            }

            player.Targets = targets;
        }

        SkillManager.Instance.UseSkill(player, packet.Info.SkillId);
    }

    public void HandleHit(Player player, C_Hit packet) //화살만
    {
        var attackId = packet.AttackId; //화살아이디
        var hitId = packet.HitId; //맞은아이디

        if (attackId == hitId || attackId == 0 || hitId == 0 || attackId == player.Id)
        {
            Console.WriteLine("----------공격 오류-----------");
            return;
        }

        Projectile projectile = null;
        if (_projectilList.TryGetValue(attackId, out projectile) == false)
        {
            Console.WriteLine("HandleHit이 Projectile아님");
            return;
        }


        if (player.Id == hitId) //본인이 맞았는데 본인이 보고할경우
        {
            if (projectile.GetOwner() == player) //본인이 본인을 때릴경우
                return;

            player.OnDamaged(projectile.GetOwner(), projectile.Attack);
            Console.WriteLine($"{projectile.GetOwner().info.Name}이 {player.info.Name}를 {projectile.Attack}만큼 공격");

            Push(LeaveGame, attackId);
        }
        else if (player.Id == attackId) //본인이 다른사람을 때린경우
        {
            if (ObjectManager.GetObjectTypeById(hitId) == GameObjectType.Monster)
            {
                Monster gameObject = null;
                if (_MonsterList.TryGetValue(hitId, out gameObject)) return;
            }
            else if (ObjectManager.GetObjectTypeById(hitId) == GameObjectType.Player)
            {
                Player gameObject = null;
                if (_playerList.TryGetValue(hitId, out gameObject)) return;
            }
        } //본인이 다른사람을 때린경우


        //Todo:
    }
}