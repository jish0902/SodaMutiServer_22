using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Game.Room;
using System;
using System.Collections.Generic;
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
                BroadCast(gameObject.CurrentPlanetId, spawnpacket);
            }


        }

        public void LeaveGame(int id)
        {
            Player player;
            if(true == _playerList.TryGetValue(id ,out player)){
                Planet planet =  Map.Planets.Find(p => p.Id == player.CurrentPlanetId);
                planet.Players.Remove(player);
               

                S_Despawn despawnpacket = new S_Despawn();
                despawnpacket.ObjcetIds.Add(id);
                BroadCast(player.CurrentPlanetId, despawnpacket);
            }

        }


        private void SetPosAndPlanetsId(Player player)
        {
            List<Planet> _planets =  Map.Planets.FindAll(p => p.isSpawnPoint);
            //임시
            player.CellPos = new Vector2(_planets[0].PosX , _planets[0].PosY + _planets[0].Round / 2 + 1);
            player.CurrentPlanetId = _planets[0].Id;

        }

    }//gameroom


    
}
