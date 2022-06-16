using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public partial class GameRoom 
    {
		bool occupationMode = false;

		public void SetGameMode()
		{
			occupationMode = true;
		}


		public void UpdateGameRole()
		{
			if (occupationMode)
			{
				List<Player> _players = Map.GetPlayerInOccupationPos(2);

			}
		}


	}
}
