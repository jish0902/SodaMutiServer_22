using System;
using System.IO;
using Newtonsoft.Json;

namespace Server.Data;

[Serializable]
public class ServerConfig
{
    public string connectionString;
    public string dataPath;
}

internal class ConfingManager
{
    public static ServerConfig config { get; private set; }

    public static void LoadConfig()
    {
        Console.WriteLine("testLenght start ");
        var t = Directory.GetCurrentDirectory();
        Console.WriteLine(t);

        var path = Path.Combine(Path.GetFullPath("./"), "config.json");


        Console.WriteLine(path);
        var text = File.ReadAllText(path); //debug파일에 있음
        Console.WriteLine($"testLenght{text.Length}");
        config = JsonConvert.DeserializeObject<ServerConfig>(text);
    }
}