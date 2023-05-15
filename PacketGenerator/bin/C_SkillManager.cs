
using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class SkillManager
{
    #region Singleton
    static SkillManager _instance = new SkillManager();
    public static SkillManager Instance { get { return _instance; } }
    #endregion

    public SkillManager()
    {
        Register();
    }

    Dictionary<int,Action<CreatureController>> _handler = new Dictionary<int,Action< CreatureController>>();


    public void Register()
    {
		_handler.Add(100, SkillHandler.Skill100);
		_handler.Add(101, SkillHandler.Skill101);
		_handler.Add(102, SkillHandler.Skill102);
		_handler.Add(103, SkillHandler.Skill103);
		_handler.Add(104, SkillHandler.Skill104);
		_handler.Add(110, SkillHandler.Skill110);
		_handler.Add(200, SkillHandler.Skill200);
		_handler.Add(201, SkillHandler.Skill201);
		_handler.Add(202, SkillHandler.Skill202);
		_handler.Add(203, SkillHandler.Skill203);
		_handler.Add(204, SkillHandler.Skill204);
		_handler.Add(205, SkillHandler.Skill205);
		_handler.Add(300, SkillHandler.Skill300);
		_handler.Add(301, SkillHandler.Skill301);
		_handler.Add(302, SkillHandler.Skill302);
		_handler.Add(303, SkillHandler.Skill303);
		_handler.Add(304, SkillHandler.Skill304);

    }

    public void UseSkill(CreatureController cc, S_Skill packet)
    {
        Action<CreatureController> action;
        if (_handler.TryGetValue(packet.Info.SkillId, out action))
            action.Invoke(cc);

    }
}

