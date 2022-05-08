
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml;
using Server;
using Server.Data;

namespace PacketGenerator
{
	class Program
	{
		static string clientRegister;
		static string serverRegister;
		static string skillRegister;
		static string skillCoolRegister;
		static string skillCoolMemberRegister;

		static void Main(string[] args)
		{
			ProtocolGen(args);
			GenSkillManager(args);
		}


		static public void ProtocolGen(string[] args)
        {
			string file = "../../../Common/protoc-3.19.3-win64/bin/Protocol.proto";
			if (args.Length >= 1)
				file = args[0];

			bool startParsing = false;
			foreach (string line in File.ReadAllLines(file))
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

				string[] names = line.Trim().Split(" =");
				if (names.Length == 0)
					continue;

				string name = names[0];
				if (name.StartsWith("S_"))
				{
					string[] words = name.Split("_");

					string msgName = "";
					foreach (string word in words)
						msgName += FirstCharToUpper(word);

					string packetName = $"S_{msgName.Substring(1)}";
					clientRegister += string.Format(PacketFormat.managerRegisterFormat, msgName, packetName);
				}
				else if (name.StartsWith("C_"))
				{
					string[] words = name.Split("_");

					string msgName = "";
					foreach (string word in words)
						msgName += FirstCharToUpper(word);

					string packetName = $"C_{msgName.Substring(1)}";
					serverRegister += string.Format(PacketFormat.managerRegisterFormat, msgName, packetName);
				}

			}

			string clientManagerText = string.Format(PacketFormat.managerFormat, clientRegister);
			File.WriteAllText("ClientPacketManager.cs", clientManagerText);
			Console.WriteLine(1);
			string serverManagerText = string.Format(PacketFormat.managerFormat, serverRegister);
			File.WriteAllText("ServerPacketManager.cs", serverManagerText);
			Console.WriteLine(1);
		}



		public static void GenSkillManager(string[] args)
        {
			string file = "../../../Common/Config/SkillData.json";
			if (args.Length >= 2)
				file = args[1];
			
			DataManager.LoadData(file);
			

			foreach (var item in DataManager.SkillDict)
            {
				skillRegister += string.Format(PacketFormat.skillManagerRegisterFormat, item.Key.ToString()) +Environment.NewLine;
				skillCoolRegister += string.Format(PacketFormat.SkillCoolHandlerReigsterFormat, item.Key.ToString()) + Environment.NewLine;
				skillCoolMemberRegister += string.Format(PacketFormat.SkillCoolMemeberReigsterFormat, item.Key.ToString()) + Environment.NewLine;
			}
			string c_skillRegisterText = string.Format(PacketFormat.c_skillHandlerFormat, skillRegister);
			string s_skillRegisterText = string.Format(PacketFormat.s_skillHandlerFormat, skillRegister);
			string SkillCoolDown = string.Format(PacketFormat.skillCoolDownFormat, skillCoolRegister, skillCoolMemberRegister);

			File.WriteAllText("C_SkillManager.cs", c_skillRegisterText);
			File.WriteAllText("S_SkillManager.cs", s_skillRegisterText);
			File.WriteAllText("SkillCoolDown.cs", SkillCoolDown);


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
}
