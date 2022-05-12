using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Game.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Server.Game
{
    public partial class GameRoom : JobSerializer
    {
        public int RoomId { get; set; }

        Dictionary<int, Player> _playerList = new Dictionary<int, Player>();
        Dictionary<int, Monster> _MonsterList = new Dictionary<int, Monster>();
        Dictionary<int, Projectile> _projectilList = new Dictionary<int, Projectile>();

        public Map Map { get; private set; } = new Map();

        public void Init(int mapId, int zoneCells)
        {
            Map.LoadMap(mapId);
            Map.SetMonster(this,1);
            
            



        }

        public void Update()
        {
            Flush();
        }

        



        public void BroadCast(int id, IMessage message)
        {
            List<Player> _players = Map.GetPlanetPlayers(id);
            if (_players == null || _players.Count <= 0)
                return;

            foreach (Player player in _players)
            {
                player.Session.Send(message);
            }
        }

        public void EnterGame(GameObject gameObject,bool randomPos)
        {
            if (gameObject == null)
                return;

            GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.Id);


            if(type == GameObjectType.Player){
                Player player = gameObject as Player;
                _playerList.Add(gameObject.Id, player);
                player.Room = this;

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

                SetPosAndPlanetsId(player);

                Map.AddObject(player);


                //본인에게 정보 전송
                {
                    S_EnterGame enterPacket = new S_EnterGame();
                    enterPacket.Player = player.info;
                    player.Session.Send(enterPacket);

                    player.Vision.Update();
                }
            }
            else if (type == GameObjectType.Monster)
            {
                Monster monster = gameObject as Monster;
                _MonsterList.Add(gameObject.Id, monster);
                monster.Room = this;

                Map.AddObject(monster);
                monster.Update();
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile projectile = gameObject as Projectile;
                _projectilList.Add(gameObject.Id, projectile);
                projectile.Room = this;

                Map.AddObject(projectile);
                projectile.Update();


            }//if끝

            {

                S_Spawn spawnpacket = new S_Spawn();
                spawnpacket.Objects.Add(gameObject.info);
                BroadCast(gameObject.CurrentRoomId, spawnpacket);
            }


        }

        public void LeaveGame(int id)
        {
            GameObjectType type = ObjectManager.GetObjectTypeById(id);


            if (type == GameObjectType.Player)
            {
                Player player;
                if (true == _playerList.TryGetValue(id, out player))
                {
                    Room.Room room = Map.Rooms.Find(p => p.Id == player.CurrentRoomId);
                    room.Players.Remove(player);

                    Map.RemoveObject(player);

                    S_Despawn despawnpacket = new S_Despawn();
                    despawnpacket.ObjcetIds.Add(id);
                    BroadCast(player.CurrentRoomId, despawnpacket);
                    _playerList.Remove(id);
                }

            }
            else if(type == GameObjectType.Monster)
            {
                Monster monster;
                if (true == _MonsterList.TryGetValue(id, out monster))
                {
                    Room.Room room = Map.Rooms.Find(p => p.Id == monster.CurrentRoomId);
                    room.Objects.Remove(monster);
                    
                    Map.RemoveObject(monster);

                    S_Despawn despawnpacket = new S_Despawn();
                    despawnpacket.ObjcetIds.Add(id);
                    BroadCast(monster.CurrentRoomId, despawnpacket);
                    _MonsterList.Remove(id);
                }

            }
            else if(type == GameObjectType.Projectile)
            {
                Projectile projectile;
                if (true == _projectilList.TryGetValue(id, out projectile))
                {
                    Room.Room room = Map.Rooms.Find(p => p.Id == projectile.CurrentRoomId);
                    room.Objects.Remove(projectile);

                    Map.RemoveObject(projectile);

                    S_Despawn despawnpacket = new S_Despawn();
                    despawnpacket.ObjcetIds.Add(id);
                    BroadCast(projectile.CurrentRoomId, despawnpacket);
                    _projectilList.Remove(id);
                }
            }

        }


        public Player FindCloestPlayer(GameObject go)
        {
            Player player = null;

            List<Player> players =  Map.GetPlanetPlayers(go.CurrentRoomId);
            if (players == null)
                return player;

            Vector2 t = new Vector2() { X = 99, Y=99 };
            foreach (Player p in players)
            {
                Vector2 temp = p.CellPos - go.CellPos;
                if(t.Length() > temp.Length())
                {
                    t = temp;
                    player = p;
                }
            }

            return player;
        }

        private void SetPosAndPlanetsId(Player player)
        {
            List<Room.Room> _rooms =  Map.Rooms.FindAll(p => p.isSpawnPoint);
            //임시
            player.CellPos = new Vector2(_rooms[0].PosX , _rooms[0].PosY);
            player.CurrentRoomId = _rooms[0].Id;

        }

    }//gameroom


    
}
