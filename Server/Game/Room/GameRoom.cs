﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Collision.Shapes;
using Google.Protobuf;
using Google.Protobuf.Protocol;

namespace Server.Game;

public partial class GameRoom : JobSerializer
{
    public int RoomId { get; set; }
    public Map Map { get; } = new();
    
    private readonly Dictionary<int, Monster> _MonsterList = new();
    private readonly Dictionary<int, Player> _playerList = new();
    private readonly Dictionary<int, SkillObj> _skillObjList = new();


    public void Init(int mapId, int zoneCells)
    {
        Map.LoadMap(mapId);
        Map.SetMonster(this, 1);

        SetGameMode();
    }

    private void ArrowTest(Player p)
    {
        for (var i = 0; i < 2; i++)
        {
            Shape t = new Polygon(0,0,);
            var arrow = ObjectManager.Instance.Add<Arrow>();
            if (arrow == null)
                return;
            arrow.CurrentRoomId = p.CurrentRoomId;
            arrow.info.Name = "Arrow";
            arrow.PosInfo.State = CreatureState.Moving;
            arrow.OwnerId = p.Id;
            arrow.PosInfo.DirX = 1;
            arrow.PosInfo.DirY = 0;
            arrow.PosInfo.PosX = 0;
            arrow.PosInfo.PosY = i;
            arrow.Speed = 20;
            arrow.info.SkillId = 200;
            arrow.Attack = 10;

            //Console.WriteLine($"P pos : {p.PosInfo.PosX},{p.PosInfo.PosY}");
            //Console.WriteLine($"arrow pos : {arrow.PosInfo.PosX},{arrow.PosInfo.PosY}");
            p.gameRoom.Push(p.gameRoom.EnterGame, arrow, false);
        }
    }


    public void Update()
    {
        Flush();
        UpdateGameRole();
    }


    public void BroadCast(int id, IMessage message)
    {
        var _players = Map.GetPlanetPlayers(id);
        if (_players == null || _players.Count <= 0)
            return;

        foreach (var player in _players) player.Session.Send(message);
    }

    public void EnterGame(GameObject gameObject, bool randomPos)
    {
        if (gameObject == null)
            return;

        var type = ObjectManager.GetObjectTypeById(gameObject.Id);


        if (type == GameObjectType.Player)
        {
            var player = gameObject as Player;
            _playerList.Add(gameObject.Id, player);
            player.gameRoom = this;

            //player.RefreshAddtionalStat();

            //TODO : 삭제
            if (player.Hp <= 0)
                player.OnDead(player);


            //GetZone(player.CellPos).Players.Add(player);
            //for (int i = 0; i < 5; i++)
            //{
            //    if (Map.ApplyMove(player, new Vector2Int(player.CellPos.x, player.CellPos.y)) == true)
            //        break;
            //}

            if (Map.SetPosAndRoomtsId(player) == false) Console.WriteLine("맵 스폰 오류");

            Map.AddObject(player);


            //본인에게 정보 전송
            {
                var enterPacket = new S_EnterGame();
                enterPacket.Player = player.info;
                player.Session.Send(enterPacket);

                player.Vision.Update();

                //--------------------------------------------
                Map.SendMapInfo(player);
            }
        }
        else if (type == GameObjectType.Monster)
        {
            var monster = gameObject as Monster;
            _MonsterList.Add(gameObject.Id, monster);
            monster.gameRoom = this;

            Map.AddObject(monster);
            monster.Update();
        }
        else if (type == GameObjectType.Projectile && type == GameObjectType.Scopeskill)
        {
            var skillObj = gameObject as SkillObj;
            _skillObjList.Add(gameObject.Id, skillObj);
            skillObj.gameRoom = this;

            Map.AddObject(skillObj);
            skillObj.Update();
        } //if끝

        {
            var spawnpacket = new S_Spawn();
            spawnpacket.Objects.Add(gameObject.info);
            BroadCast(gameObject.CurrentRoomId, spawnpacket);

            var ChangePacket = new S_ChangeHp();
            ChangePacket.ObjectId = gameObject.Id;
            ChangePacket.Hp = gameObject.Hp;
            BroadCast(gameObject.CurrentRoomId, ChangePacket);
        }
    }

    public void LeaveGame(int id)
    {
        var type = ObjectManager.GetObjectTypeById(id);


        if (type == GameObjectType.Player)
        {
            Player player;
            if (_playerList.TryGetValue(id, out player))
            {
                if (Map.RemoveObject(player) == -1)
                    Console.WriteLine("지우기 오류");

                var despawnpacket = new S_Despawn();
                despawnpacket.ObjcetIds.Add(id);
                BroadCast(player.CurrentRoomId, despawnpacket);
                _playerList.Remove(id);
            }
        }
        else if (type == GameObjectType.Monster)
        {
            Monster monster;
            if (_MonsterList.TryGetValue(id, out monster))
            {
                if (Map.RemoveObject(monster) == -1)
                    Console.WriteLine("지우기 오류");
                ;

                var despawnpacket = new S_Despawn();
                despawnpacket.ObjcetIds.Add(id);
                BroadCast(monster.CurrentRoomId, despawnpacket);
                _MonsterList.Remove(id);
            }
        }
        else if (type == GameObjectType.Projectile || type == GameObjectType.Scopeskill)
        {
            SkillObj skillObj;
            if (_skillObjList.TryGetValue(id, out skillObj))
            {
                if (Map.RemoveObject(skillObj) == -1)
                    Console.WriteLine("지우기 오류");
                skillObj.Destroy();
                var despawnpacket = new S_Despawn();
                despawnpacket.ObjcetIds.Add(id);
                BroadCast(skillObj.CurrentRoomId, despawnpacket);
                _skillObjList.Remove(id);
            }
        }
    }


    public Player FindCloestPlayer(GameObject go, int[] except = null)
    {
        Player player = null;

        var players = Map.GetPlanetPlayers(go.CurrentRoomId).Where(t => t.Id != go.Id).ToList();
        if (players == null)
            return player;

        if (players.Count() == 0)
            return player;

        var t = new Vector2 { X = 99, Y = 99 };
        foreach (var p in players)
        {
            if (except != null)
                if (except.Contains(p.Id) || except.Contains(p.OwnerId))
                    continue;
            ;


            var temp = p.CellPos - go.CellPos;
            if (t.Length() > temp.Length())
            {
                t = temp;
                player = p;
            }
        }

        return player;
    }

    public Monster FindCloestMonster(GameObject go, int[] except = null)
    {
        Monster monster = null;

        var monsters = Map.GetPlanetObjects(go.CurrentRoomId).Where(i => i.ObjectType == GameObjectType.Monster)
            .ToList();

        if (monsters == null)
            return monster;

        var t = new Vector2 { X = 99, Y = 99 };
        foreach (Monster p in monsters)
        {
            if (except != null)
                if (except.Contains(p.Id) || except.Contains(p.OwnerId))
                    continue;
            ;

            var temp = p.CellPos - go.CellPos;
            if (t.Length() > temp.Length())
            {
                t = temp;
                monster = p;
            }
        }

        return monster;
    }


    public GameObject FindCloestMonsterAndPlayer(GameObject go, int[] except = null)
    {
        GameObject results = null;

        var _targets = Map.GetPlanetObjects(go.CurrentRoomId).Where(i => i.ObjectType == GameObjectType.Monster)
            .ToList();
        _targets.AddRange(Map.GetPlanetPlayers(go.CurrentRoomId).Where(t => t.Id != go.Id).ToList());


        if (_targets == null)
            return results;

        var t = new Vector2 { X = 99, Y = 99 };
        foreach (var p in _targets)
        {
            if (except != null)
                if (except.Contains(p.Id) || except.Contains(p.OwnerId))
                    continue;
            ;

            var temp = p.CellPos - go.CellPos;
            if (t.Length() > temp.Length())
            {
                t = temp;
                results = p;
            }
        }

        return results;
    }
} //gameroom