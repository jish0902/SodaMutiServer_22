﻿using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Server.Game.Room
{
	//public class Planet
	//   {
	//	public int Count;
	//	public int PosX { get; set; }
	//	public int PosY { get; set; }
	//	public int Round { get; set; }
	//	public int Id { get; set; }
	//	public bool isSpawnPoint { get; set; }
	//	public List<int> TouarableIds { get; set; } = new List<int>();
	//	public List<Player> Players { get; set; } = new List<Player>();
	//	public HashSet<GameObject> Objects { get; set; } = new HashSet<GameObject>();
	//}

	public class Room
	{
		public bool[,] Collisions { get; set; }
		public int PosX { get; set; }
		public int PosY { get; set; }
		public int RoomType { get; set; }
		public int RoomTempletId { get; set; }
		public int Id { get; set; }
		public bool isSpawnPoint { get; set; }
		public List<Player> Players { get; set; } = new List<Player>();
		public List<Room> TouarableRooms { get; set; } = new List<Room>();
		public HashSet<GameObject> Objects { get; set; } = new HashSet<GameObject>();
	}

	

	public class Map
    {
		
		public List<Room> Rooms { get; set; } = new List<Room>();

		public void LoadMap(int mapId, string pathPrefix = "../../../../../Common/MapData")
		{
			int Distance = 44;


			//----------------------------------------
			string mapName = "Map_" + mapId.ToString("000");

			// Collision 관련 파일
			string text = File.ReadAllText($"{pathPrefix}/{mapName}.txt");
			StringReader reader = new StringReader(text);

			int childCount = int.Parse(reader.ReadLine());

            for (int i = 0; i < childCount; i++)
            {
				string[] rInfo = reader.ReadLine().Split('/');  //x,y,roomtype,roomtempletType  roomttype 1:스폰2:통로3:일반 
																//roomtempletType 1:1번형태의 방 2:2번형태
				
				if (rInfo.Length == 5)                               
				{
					Room room = new Room();

					room.PosX = int.Parse(rInfo[0]);
					room.PosY = int.Parse(rInfo[1]);

					if (int.Parse(rInfo[2]) == 1)
						room.isSpawnPoint = true;
					room.RoomType = int.Parse(rInfo[2]); // 1,2 방 3 길
					room.RoomTempletId = int.Parse(rInfo[3]); //1 기본 2 특수
					room.Id = int.Parse(rInfo[4]);
					Rooms.Add(room);   
				}
				else if(rInfo.Length == 4)
                {
					Room room = new Room();

					room.PosX = int.Parse(rInfo[0]);
					room.PosY = int.Parse(rInfo[1]);

					if (int.Parse(rInfo[2]) == 1)
						room.isSpawnPoint = true;
					room.RoomType = int.Parse(rInfo[2]); // 1,2 방 3 길
					Rooms.Add(room);
				}
                else
                {
                    Console.WriteLine("맵 로드 실패  : 인자의 길이가 맞지 않음");
                }
			} // 방끝

            foreach (Room r in Rooms)
            {
				Vector2 main = new Vector2(r.PosX, r.PosY);
				foreach (Room next in Rooms)
				{
					if (r == next)
						continue;

					Vector2 sub = new Vector2(next.PosX, next.PosY);
					if (Vector2.Distance(main, sub) < Distance + 1)
					{
						r.TouarableRooms.Add(next);
					}
				}

			} //투어 끝


            //for (int i = 0; i < childCount; i++)
            //{
            //    int MinX = int.Parse(reader.ReadLine());
            //    int MaxX = int.Parse(reader.ReadLine());
            //    int MinY = int.Parse(reader.ReadLine());
            //    int MaxY = int.Parse(reader.ReadLine());

            //    int xCount = MaxX - MinX + 1;
            //    int yCount = MaxY - MinY + 1;
            //    Rooms[i].Collisions = new bool[yCount, xCount];

            //    for (int y = 0; y < yCount; y++)
            //    {
            //        string line = reader.ReadLine();
            //        for (int x = 0; x < xCount; x++)
            //        {
            //            Rooms[i].Collisions[y, x] = (line[x] - '0') > 0;
            //            //Console.Write(Rooms[i].Collisions[y, x] ? '1' :'0');
            //        }
            //    }

            //}



        }//LoadMap



		public void SetMonster(GameRoom room,int monsterCount)
        {
            //foreach (Room p in Rooms)
            //{
            //    for (int i = 0; i < monsterCount; i++)
            //    {
            //        //나중에는 맵의 불,물,지옥 지형에 따라 몬스터 class로 이런식으로 렌덤값 추출
            //        Random rand = new Random();
            //        int r = rand.Next(1, DataManager.MonsterDict.Count + 1);
            //        Monster monster = ObjectManager.Instance.Add<Monster>();
            //        {
            //            monster.CurrentPlanetId = p.Id;
            //            monster.Init(r); //side , posinfo

            //            int t = monster.Side;
            //            float small = 0.3f;
            //            if (t == 0)
            //            {
            //                monster.PosInfo.PosX = p.PosX + rand.Next(-(p.Round / 2 - 1), p.Round / 2 - 1);
            //                monster.PosInfo.PosY = p.PosY + p.Round / 2 + small;
            //            }
            //            else if (t == 1)
            //            {
            //                monster.PosInfo.PosX = p.PosX + p.Round / 2 + small;
            //                monster.PosInfo.PosY = p.PosY + rand.Next(-(p.Round / 2 - 1), p.Round / 2 - 1);
            //            }
            //            else if (t == 2)
            //            {
            //                monster.PosInfo.PosX = p.PosX + rand.Next(-(p.Round / 2 - 1), p.Round / 2 - 1);
            //                monster.PosInfo.PosY = p.PosY - p.Round / 2 - small;
            //            }
            //            else if (t == 3)
            //            {
            //                monster.PosInfo.PosX = p.PosX - p.Round / 2 - small;
            //                monster.PosInfo.PosY = p.PosY + rand.Next(-(p.Round / 2 - 1), p.Round / 2 - 1);
            //            }
            //            else
            //            {
            //                Console.WriteLine("GetPlanetRotationById 오류");
            //            }



            //        };



            //        room.Push(room.EnterGame, monster, false);
            //        //p.Objects.Add(monster);

            //    }

            //}

        }

		
		public void AddObject(GameObject go)
        {
            if (go.ObjectType == GameObjectType.Player)
            {
				Room planet = Rooms.Find(p => { return p.Id == (go.CurrentPlanetId); });
				if(planet == null || planet.Players.Contains((Player)go))
					return;
				planet.Players.Add((Player)go);
			}
			else 
            {
				Room planet = Rooms.Find(p => { return p.Id == (go.CurrentPlanetId); });
				if (planet == null || planet.Objects.Contains(go))
					return;
				planet.Objects.Add(go);
			}
        }
   

		public int RemoveObject(GameObject go)
        {
			if (go.ObjectType == GameObjectType.Player)
			{
				Room plant = Rooms.Find(p => { return p.Id == go.CurrentPlanetId; });
				if (plant == null || plant.Players.Contains((Player)go))
					return -1;

				plant.Players.Remove((Player)go);
				return go.Id;
			}
			else
			{
				Room plant = Rooms.Find(p => { return p.Id == go.CurrentPlanetId; });
				if (plant == null || plant.Objects.Contains(go))
					return -1;

				plant.Objects.Remove(go);
				return go.Id;
			}
		}

		public List<Player> GetPlanetPlayers(int id, int level = 1)
		{
			if (id == -1)
				return null;

			Room plant = Rooms.Find(p => { return p.Id == id; });

			if (plant == null)
			{
				Console.WriteLine("행성 아이디 없음 오류");
				return null;
			}

			List<Player> _players = plant.Players;

			if (level == 1) //현제의 오브젝트와 가는 중에 오브젝트 가져오기
			{
				//할일   
			}
			else if (level == 2) //완전히 갈수있는곳의 오브젝트 전부 가져오기
			{
				foreach (Room r in plant.TouarableRooms)
				{
					Room _room = Rooms.Find(p => { return p.Id == r.Id; });
					_players.AddRange(_room.Players);
				}
			}

			return _players;
		}

		public HashSet<GameObject> GetPlanetObjects(int id, int level = 1)
		{
			if (id == -1)
				return null;

			Room plant = Rooms.Find(p => { return p.Id == id; });

			if (plant == null)
			{
				Console.WriteLine("행성 아이디 없음 오류");
				return null;
			}

			HashSet<GameObject> _objects = new HashSet<GameObject>();
			_objects.UnionWith(plant.Objects);

			if (level == 1) //현제의 오브젝트와 가는 중에 오브젝트 가져오기
			{
				//할일   
			}
			else if (level == 2) //완전히 갈수있는곳의 오브젝트 전부 가져오기
			{
				foreach (Room room in plant.TouarableRooms)
				{
					Room _room = Rooms.Find(p => { return p.Id == room.Id; });
					_objects.UnionWith(_room.Objects);
				}
			}

			return _objects;
		}

		public void ApplyProjectile(Vector2 pos,Vector2 dir)
        {
			 //dir - pos

		}





	} //class


}