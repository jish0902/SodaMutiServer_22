using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;







namespace Server.Game
{ 
	public enum RoomType  //TODO : 서보와 자동화
	{
		SPAWN = 0,
		ROOM = 1,
		PATH = 2,
		BOSSROOM = 3,
		PLAYEROWNROOM = 4,

	}


	public class Room
	{
		/*$"{tr.position.x}/{tr.position.y}/1/{room.RoomTypeId}/{room.RoomId}";
		 x,y,roomtype,roomtempletType  roomttype 1:스폰 2:일반 3:통로
		roomtempletType 1:1번형태의 방 2:2번형태

		 */
		public bool[,] Collisions { get; set; }
		public float PosX { get; set; }
		public float PosY { get; set; }
		public int Id { get; set; }
		public RoomType RoomType { get; set; }
		public int RoomLevel { get; set; } = 0;
		public int RoomskinId { get; set; } = 0;
		public bool isSpawnPoint { get { return RoomType == RoomType.SPAWN; } }
		public List<Player> Players { get; private set; } = new List<Player>();
		public List<Room> TouarableRooms { get; private set; } = new List<Room>();
		public HashSet<GameObject> Objects { get; private set; } = new HashSet<GameObject>();
	}




	public class Map 
    {
		public Vector2Int Bleft { get; private set; }
		public Vector2Int Tright { get; private set; }


		public int[,] _collisions;
		private List<Room> Rooms { get;  set; } = new List<Room>();  //요기서만 사용

		int roomSize = 0;
		public void LoadMap(int mapId, string pathPrefix = "../../../../../Common/MapData")
		{
			int Distance = 22;


			//----------------------------------------
			string mapName = "Map_" + mapId.ToString("000");

			// Collision 관련 파일
			string text = File.ReadAllText($"{pathPrefix}/{mapName}.txt");
			StringReader reader = new StringReader(text);

			int roomCount = int.Parse(reader.ReadLine());

            for (int i = 0; i < roomCount; i++)
            {
				string[] rInfo = reader.ReadLine().Split('/');  //x,y,roomtype,roomtempletType  roomttype 1:스폰2:통로3:일반 
																//roomtempletType 1:1번형태의 방 2:2번형태
				
				if (rInfo.Length == 5)                               
				{
					//Room room = new Room();

					//room.PosX = int.Parse(rInfo[0]);
					//room.PosY = int.Parse(rInfo[1]);

					//if (int.Parse(rInfo[2]) == 1)
					//	room.isSpawnPoint = true;
					//room.RoomTypeId = int.Parse(rInfo[2]); // 1,2 방 3 길
					//room.RoomskinId = int.Parse(rInfo[3]); //1 기본 2 특수
					//room.Id = int.Parse(rInfo[4]);
					//Rooms.Add(room);   
				}
				else if(rInfo.Length == 4)
                {
					Room room = new Room();

					room.PosX = float.Parse(rInfo[0]);
					room.PosY = float.Parse(rInfo[1]);
					room.RoomType = (RoomType)Enum.Parse(typeof(RoomType), rInfo[2]);
					room.Id = int.Parse(rInfo[3]);

					Rooms.Add(room);
				}
                else
                {
                    Console.WriteLine("맵 로드 실패  : 인자의 길이가 맞지 않음");
                }
			} // 방끝


			string[] minIndex = reader.ReadLine().Split('/');

			roomSize = int.Parse(reader.ReadLine());

			Bleft = new Vector2Int(int.Parse(minIndex[0]), int.Parse(minIndex[1]));
			Tright = new Vector2Int(Bleft.x + (roomSize - 1), Bleft.y + (roomSize - 1));


			_collisions = new int[roomSize, roomSize];


			for (int x = 0; x < roomSize; x++)
            {
				Buffer.BlockCopy(
					 Array.ConvertAll<string, int>(reader.ReadLine().Split(','), s => int.Parse(s)),
					 0, _collisions, x * roomSize * sizeof(int), roomSize * sizeof(int));
			}

            //for (int i = 0; i < roomSize; i++)                  디버깅
            //{
            //    for (int j = 0; j < roomSize; j++)
            //    {
            //        Console.Write(MapArray[i, j]);
            //    }
            //    Console.WriteLine();
            //}



            foreach (Room r in Rooms)
            {
				
				Vector2 main = new Vector2(r.PosX, r.PosY);
				foreach (Room next in Rooms)
				{
					if (r == next)
						continue;
					if (r.RoomType == next.RoomType)
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


   //         while (true)
   //         {
			//	int x = int.Parse(Console.ReadLine());
			//	int y = int.Parse(Console.ReadLine());
			//	bool k = CanGo(new Vector2Int(x, y), false);
   //             Console.WriteLine(k);
			//}

			

		}//LoadMap



		public void SetMonster(GameRoom room,int monsterCount)
        {

			int[] CanSpwanRandomMonsterArr = DataManager.MonsterDict.Keys.Where(i => i < 50).ToArray<int>();

			foreach (Room r in Rooms)
			{
				if (r.RoomType == RoomType.PATH)
					continue;

				for (int i = 0; i < monsterCount; i++)
				{
					Random rand = new Random();
					
					int rint = rand.Next(0, CanSpwanRandomMonsterArr.Count());
					Monster monster = ObjectManager.Instance.Add<Monster>();
					{
						monster.CurrentRoomId = r.Id;
						monster.Init(CanSpwanRandomMonsterArr[rint]); //side , posinfo

						int Round = 10; //temp 나중에 받아와야함

						monster.PosInfo.PosX = r.PosX + rand.Next(-Round, Round);
						monster.PosInfo.PosY = r.PosY + rand.Next(-Round, Round);
					}

					room.Push(room.EnterGame, monster, false);

				} //갯수마다

			}//	방마다

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
				Room room = Rooms.Find(p => { return p.Id == (go.CurrentRoomId); });
				if(room == null || room.Players.Contains((Player)go))
					return;
				room.Players.Add((Player)go);
			}
			else 
            {
				Room room = Rooms.Find(p => { return p.Id == (go.CurrentRoomId); });
				if (room == null || room.Objects.Contains(go))
					return;
				room.Objects.Add(go);
			}
        }
		
		public GameObject FindObjById(int roomId,int playerId,int level = 2)
        {
			List<Player> t = GetPlanetPlayers(roomId, level);
			GameObject go = t.Find(p => p.Id == playerId);
			return go;
		}

		public int RemoveObject(GameObject go)
        {
			if (go.ObjectType == GameObjectType.Player)
			{
				Room room = Rooms.Find(r => { return r.Id == go.CurrentRoomId; });
				if (room == null || room.Players.Contains((Player)go) == false)
					return -1;

				room.Players.Remove((Player)go);
				return go.Id;
			}
			else
			{
				Room room = Rooms.Find(r => { return r.Id == go.CurrentRoomId; });
				if (room == null || room.Objects.Contains(go) == false)
					return -1;

				room.Objects.Remove(go);
				return go.Id;
			}
		}


		public List<Player> GetPlanetPlayers(int roomId, int level = 1)
		{
			if (roomId == -1)
				return null;

			Room room = Rooms.Find(r => { return r.Id == roomId; });

			if (room == null)
			{
				Console.WriteLine("행성 아이디 없음 오류");
				return null;
			}

			List<Player> _players = room.Players;

			if (level == 1) //현제의 오브젝트와 가는 중에 오브젝트 가져오기
			{
				//할일   
			}
			else if (level == 2) //완전히 갈수있는곳의 오브젝트 전부 가져오기
			{
				foreach (Room r in room.TouarableRooms)
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

		public bool SetPosAndRoomtsId(Player player)
		{
			List<Room> _rooms = Rooms.FindAll(p => p.isSpawnPoint);
			//임시

			if (_rooms == null || _rooms.Count == 0)
				return false;

			player.CellPos = new Vector2(_rooms[0].PosX, _rooms[0].PosY);
			player.CurrentRoomId = _rooms[0].Id;
			return true;
		}

		public void MoveRoom(GameObject Target,int NextRoom)
        {
			int now = Target.CurrentRoomId;

			Room nowRoom = GetRoom(now);
			if (nowRoom == null)
			{
				Console.WriteLine("방 오류");
				return;
			}
			Room nextRoom = nowRoom.TouarableRooms.Find(t => t.Id == NextRoom);
			if (nextRoom == null)
			{
				Console.WriteLine($"이동 오류{nowRoom.TouarableRooms}");
				return;
			}

			if(RemoveObject(Target) == 1)
				 Console.WriteLine("지우기 오류");

			Target.CurrentRoomId = NextRoom;
			AddObject(Target);


            //----------------- 자신의 몬스터이동 -------------------
            foreach (GameObject ownMonster in GetPlanetObjects(now).Where(obj => obj.OwnerId == Target.Id).ToArray())
            {
				if(RemoveObject(ownMonster) == -1)
					Console.WriteLine("지우기 오류");

				ownMonster.CurrentRoomId = NextRoom;
				AddObject(ownMonster);
			} 

			//-------------- 몬스터 이동끝-----------------------------


			//디버그
			Console.WriteLine($"{Target.info.Name}이 {now}에서 {NextRoom}로 이동");
			Console.WriteLine($"{nextRoom.Objects.Count}");
			foreach (GameObject go in nextRoom.Objects)
			{
				Console.WriteLine($"{go.CellPos}{go.CurrentRoomId}{go.State}");
			}


		}

		public void ApplyProjectile(Vector2 pos,Vector2 dir)
        {
			 //dir - pos

		}

		public Room GetRoom(int id)
        {
			return Rooms.Find(r => r.Id == id);

		}

		public void SendMapInfo(Player p)
        {
			S_RoomInfo roomPacket = new S_RoomInfo();
            foreach (Room room in Rooms)
            {
				RoomInfo roomInfo = new RoomInfo();
				roomInfo.RoomId = room.Id;
				roomInfo.RoomLevel = room.RoomLevel;
				roomInfo.RoomType = (int)room.RoomType;
				roomInfo.PosX = room.PosX;
				roomInfo.PosY = room.PosY;
				roomPacket.RoomInfos.Add(roomInfo);
			}

			p.Session.Send(roomPacket);
        }





		#region Map Collison
	

		GameObject[,] _objects;
		public bool ApplyMove(GameObject gameObject, Vector2Int dest, bool cheakObjects = true, bool collision = true)
        {
			if(CanGo(dest,false) == true)
            {
                Console.WriteLine($"{dest.x}{dest.y} 이동가능");
				return true;
            }
			Console.WriteLine($"{dest.x}{dest.y} 이동불가능");
			return false;

		}

		public bool CanGo(Vector2Int cellPos, bool cheakObjects = true)
		{
			if (cellPos.x < Bleft.x || cellPos.x > Tright.x)
				return false;
			if (cellPos.y < Bleft.y || cellPos.y > Tright.y)
				return false;

			int x = cellPos.x - Bleft.x;	
			int y = cellPos.y - Bleft.y;

			int k =_collisions[x, y];
			return _collisions[x, y] > 0 && (!cheakObjects || _objects[x, y] == null);
		}



        #endregion
        //public GameObject FindByDir(GameObject go)
        //      {
        //	GameObject target = null;

        //	if (go == null)
        //		return null;
        //	Player p = go as Player;
        //	if (p == null || p.Room == null)
        //		return null;


        //	return target;

        //}


    } //class


}
