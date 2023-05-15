using System;

namespace JsonMerge;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello World!");
    }


    // public static void GenSkillManager(string[] args)
    // {
    //     string file = "../../../Common/Config/SkillData.json";
    //     if (args.Length >= 2)
    //         file = args[1];
    //
    //     DataManager.LoadData(file);
    //
    //
    //     foreach (var item in DataManager.SkillDict)
    //     {
    //         skillRegister += string.Format(PacketFormat.skillManagerRegisterFormat, item.Key.ToString()) +Environment.NewLine;
    //         skillCoolRegister += string.Format(PacketFormat.SkillCoolHandlerReigsterFormat, item.Key.ToString()) + Environment.NewLine;
    //         skillCoolMemberRegister += string.Format(PacketFormat.SkillCoolMemeberReigsterFormat, item.Key.ToString()) + Environment.NewLine;
    //     }
    //     string c_skillRegisterText = string.Format(PacketFormat.c_skillHandlerFormat, skillRegister);
    //     string s_skillRegisterText = string.Format(PacketFormat.s_skillHandlerFormat, skillRegister);
    //     string SkillCoolDown = string.Format(PacketFormat.skillCoolDownFormat, skillCoolRegister, skillCoolMemberRegister);
    //
    //     File.WriteAllText("C_SkillManager.cs", c_skillRegisterText);
    //     File.WriteAllText("S_SkillManager.cs", s_skillRegisterText);
    //     File.WriteAllText("SkillCoolDown.cs", SkillCoolDown);
    //
    //
    // }
}