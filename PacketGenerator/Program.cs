using System;
using System.IO;
using Server.Data;

namespace PacketGenerator;

internal class Program
{
    private static string clientRegister;
    private static string serverRegister;
    private static string skillRegister;
    private static string skillCoolRegister;
    private static string skillCoolMemberRegister;

    private static void Main(string[] args)
    {
        //절대 생성 금지  ../../../Common/protoc-21.3-osx-x86_64/bin 위치에서 생성하는 파일 있음
        //if (args.Length == 0)
        //	throw new Exception("실행 금지 :Common/protoc-21.3-osx-x86_64/bin 위치에서 생성하는 파일 있음 ");
        ProtocolGen(args);
        GenSkillManager(args);
    }


    public static void ProtocolGen(string[] args)
    {
        var file = "../../../Common/protoc-21.3-osx-x86_64/bin/Protocol.proto";
        if (args.Length >= 1)
            file = args[0];

        var startParsing = false;
        foreach (var line in File.ReadAllLines(file))
        {
            if (!startParsing && line.Contains("enum MsgId"))
            {
                startParsing = true;
                continue;
            }

            if (!startParsing)
                continue;

            if (line.Contains("}"))
                break;

            var names = line.Trim().Split(" =");
            if (names.Length == 0)
                continue;

            var name = names[0];
            if (name.StartsWith("S_"))
            {
                var words = name.Split("_");

                var msgName = "";
                foreach (var word in words)
                    msgName += FirstCharToUpper(word);

                var packetName = $"S_{msgName.Substring(1)}";
                clientRegister += string.Format(PacketFormat.managerRegisterFormat, msgName, packetName);
            }
            else if (name.StartsWith("C_"))
            {
                var words = name.Split("_");

                var msgName = "";
                foreach (var word in words)
                    msgName += FirstCharToUpper(word);

                var packetName = $"C_{msgName.Substring(1)}";
                serverRegister += string.Format(PacketFormat.managerRegisterFormat, msgName, packetName);
            }
        }

        var clientManagerText = string.Format(PacketFormat.managerFormat, clientRegister);
        File.WriteAllText("ClientPacketManager.cs", clientManagerText);
        Console.WriteLine(1);
        var serverManagerText = string.Format(PacketFormat.managerFormat, serverRegister);
        File.WriteAllText("ServerPacketManager.cs", serverManagerText);
        Console.WriteLine(1);
    }


    public static void GenSkillManager(string[] args)
    {
        var file = "../../../Common/Config/SkillData.json";
        if (args.Length >= 2)
            file = args[1];

        DataManager.LoadData(file);


        foreach (var item in DataManager.SkillDict)
        {
            skillRegister += string.Format(PacketFormat.skillManagerRegisterFormat, item.Key.ToString()) +
                             Environment.NewLine;
            skillCoolRegister += string.Format(PacketFormat.SkillCoolHandlerReigsterFormat, item.Key.ToString()) +
                                 Environment.NewLine;
            skillCoolMemberRegister += string.Format(PacketFormat.SkillCoolMemeberReigsterFormat, item.Key.ToString()) +
                                       Environment.NewLine;
        }

        var c_skillRegisterText = string.Format(PacketFormat.c_skillHandlerFormat, skillRegister);
        var s_skillRegisterText = string.Format(PacketFormat.s_skillHandlerFormat, skillRegister);
        var SkillCoolDown = string.Format(PacketFormat.skillCoolDownFormat, skillCoolRegister, skillCoolMemberRegister);

        File.WriteAllText("C_SkillManager.cs", c_skillRegisterText);
        File.WriteAllText("S_SkillManager.cs", s_skillRegisterText);
        File.WriteAllText("SkillCoolDown.cs", SkillCoolDown);
    }

    public static void MonstaDataMerge(string[] args)
    {
    }

    public static void SkillCoolGen(string[] args)
    {
    }


    public static string FirstCharToUpper(string input)
    {
        if (string.IsNullOrEmpty(input))
            return "";
        return input[0].ToString().ToUpper() + input.Substring(1).ToLower();
    }
}