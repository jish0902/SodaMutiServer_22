using Google.Protobuf;
using Google.Protobuf.Protocol;
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
            Map.SetMonster(this,0);


            //Arrow arrow = ObjectManager.Instance.Add<Arrow>();
            //arrow.Speed = 20;
            //arrow.CellPos = new System.Numerics.Vector2(0, 0);
            //arrow.Dir = new System.Numerics.Vector2(1, 0);
            //arrow.Room = this;
            //EnterGame(arrow,false);

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

                if(Map.SetPosAndRoomtsId(player) == false)
                {
                    Console.WriteLine("맵 스폰 오류");
                }

                Map.AddObject(player);


                //본인에게 정보 전송
                {
                    S_EnterGame enterPacket = new S_EnterGame();
                    enterPacket.Player = player.info;
                    player.Session.Send(enterPacket);

                    player.Vision.Update();

                    //--------------------------------------------
                    Map.SendMapInfo(player);
                }
            }
            else if (type == GameObjectType.Monster)
            {
                Monster monster = gameObject as Monster;
                _MonsterList.Add(gameObject.Id, monster);
                monster.gameRoom = this;

                Map.AddObject(monster);
                monster.Update();
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile projectile = gameObject as Projectile;
                _projectilList.Add(gameObject.Id, projectile);
                projectile.gameRoom = this;

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
                    if(Map.RemoveObject(player) == -1)
                        Console.WriteLine("지우기 오류");

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
                    if(Map.RemoveObject(monster) == -1)
                        Console.WriteLine("지우기 오류"); ;

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
                    if(Map.RemoveObject(projectile) == 1)
                        Console.WriteLine("지우기 오류");
                    S_Despawn despawnpacket = new S_Despawn();
                    despawnpacket.ObjcetIds.Add(id);
                    BroadCast(projectile.CurrentRoomId, despawnpacket);
                    _projectilList.Remove(id);
                }
            }

        }



        public Player FindCloestPlayer(GameObject go,int[] except = null)
        {
            Player player = null;

            List<Player> players =  Map.GetPlanetPlayers(go.CurrentRoomId);
            if (players == null)
                return player;
            
            if(players.Count() == 0)
                return player;

            Vector2 t = new Vector2() { X = 99, Y=99 };
            foreach (Player p in players)
            {
                if (except != null)
                    if (except.Contains(p.Id) || except.Contains(p.OwnerId))
                        continue; ;
              

                Vector2 temp = p.CellPos - go.CellPos;
                if(t.Length() > temp.Length())
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

            List<GameObject> monsters = Map.GetPlanetObjects(go.CurrentRoomId).Where(i => i.ObjectType == GameObjectType.Monster).ToList();
            
            if (monsters == null)
                return monster;

            Vector2 t = new Vector2() { X = 99, Y = 99 };
            foreach (Monster p in monsters)
            {
                if (except != null && except.Contains(p.Id) || except.Contains(p.OwnerId))
                    continue;

                Vector2 temp = p.CellPos - go.CellPos;
                if (t.Length() > temp.Length())
                {
                    t = temp;
                    monster = p;
                }
            }

            return monster;
        }






    }//gameroom


    
}
