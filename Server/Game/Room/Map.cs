using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Server.Game.Room
{
	public class Planet
    {
		public int Count;
		public int PosX { get; set; }
		public int PosY { get; set; }
		public int Round { get; set; }
		public int Id { get; set; }
		public bool isSpawnPoint { get; set; }
		public List<int> TouarableIds { get; set; } = new List<int>();
		public List<Player> Players { get; set; } = new List<Player>();
		public HashSet<GameObject> Objects { get; set; } = new HashSet<GameObject>();
	}


    public class Map
    {
		public int Size { get; set; }
		
		public List<Planet> Planets { get; set; } = new List<Planet>();

		public void LoadMap(int mapId, string pathPrefix = "../../../../../Common/MapData")
		{
			string mapName = "Map_" + mapId.ToString("000");

			// Collision 관련 파일
			string text = File.ReadAllText($"{pathPrefix}/{mapName}.txt");
			StringReader reader = new StringReader(text);

			Size = int.Parse(reader.ReadLine());
			
			int childCount = int.Parse(reader.ReadLine());

            for (int i = 0; i < childCount; i++)
            {
				string[] pInfo = reader.ReadLine().Split('/');  //x,y,r,id,touarableids
				if(pInfo.Length == 6)
                {
					Planet planet = new Planet();

					planet.PosX = int.Parse(pInfo[0]);
					planet.PosY = int.Parse(pInfo[1]);
					planet.Round = int.Parse(pInfo[2]);
					planet.Id = int.Parse(pInfo[3]);
					planet.isSpawnPoint = bool.Parse(pInfo[4]);

                    foreach (string t in pInfo[5].Split('_'))
                    {
						planet.TouarableIds.Add(int.Parse(t));
					}

					Planets.Add(planet);
				}
                else
                {
                    Console.WriteLine("맵 로드 실패  : 인자의 길이가 맞지 않음");
                }
			}

		}//LoadMap
		
		public void AddObject(GameObject go)
        {
            if (go.ObjectType == GameObjectType.Player)
            {
				Planet planet = Planets.Find(p => { return p.Id == go.CurrentPlanetId; });
				if(planet == null || planet.Players.Contains((Player)go))
					return;
				planet.Players.Add((Player)go);
			}
			else 
            {
				Planet planet = Planets.Find(p => { return p.Id == go.CurrentPlanetId; });
				if (planet == null || planet.Objects.Contains(go))
					return;
				planet.Objects.Add(go);
			}
        }

		public int RemoveObject(GameObject go)
        {
			if (go.ObjectType == GameObjectType.Player)
			{
				Planet plant = Planets.Find(p => { return p.Id == go.CurrentPlanetId; });
				if (plant == null || plant.Players.Contains((Player)go))
					return -1;

				plant.Players.Remove((Player)go);
				return go.Id;
			}
			else
			{
				Planet plant = Planets.Find(p => { return p.Id == go.CurrentPlanetId; });
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

			Planet plant = Planets.Find(p => { return p.Id == id; });

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
				foreach (int _id in plant.TouarableIds)
				{
					Planet _plant = Planets.Find(p => { return p.Id == _id; });
					_players.AddRange(_plant.Players);
				}
			}

			return _players;
		}

		public HashSet<GameObject> GetPlanetObjects(int id, int level = 1)
		{
			if (id == -1)
				return null;

			Planet plant = Planets.Find(p => { return p.Id == id; });

			if (plant == null)
			{
				Console.WriteLine("행성 아이디 없음 오류");
				return null;
			}

			HashSet<GameObject> _objects = new HashSet<GameObject>();
			_objects.Union(plant.Objects);

			if (level == 1) //현제의 오브젝트와 가는 중에 오브젝트 가져오기
			{
				//할일   
			}
			else if (level == 2) //완전히 갈수있는곳의 오브젝트 전부 가져오기
			{
				foreach (int _id in plant.TouarableIds)
				{
					Planet _plant = Planets.Find(p => { return p.Id == _id; });
					_objects.Union(_plant.Objects);
				}
			}

			return _objects;
		}
	}
}
