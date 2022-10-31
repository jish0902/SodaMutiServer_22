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
            Console.WriteLine($"testLenght start ");
            var t = Directory.GetCurrentDirectory();
            Console.WriteLine(t);

            var path = Path.Combine(Path.GetFullPath("./"), "config.json");
            
            
            Console.WriteLine(path);
            string text = File.ReadAllText(path); //debug파일에 있음
            Console.WriteLine($"testLenght{text.Length}");
            config = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerConfig>(text);
        }

    }
}
