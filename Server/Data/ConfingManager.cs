using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Server.Data
{

    [Serializable]
    public class ServerConfig
    {
        public string dataPath;
        public string connectionString;
    }

    class ConfingManager
    {
        public static ServerConfig config { get; private set; }

        public static void LoadConfig()
        {
            string text = File.ReadAllText("config.json"); //debug파일에 있음
            config = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerConfig>(text);
        }

    }
}
