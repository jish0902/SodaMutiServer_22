
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


		
	_handler.Add(500, SkillHandler.Skill500);


		
	_handler.Add(501, SkillHandler.Skill501);



    }

    public void UseSkill(CreatureController cc, S_Skill packet)
    {
        Action<CreatureController> action;
        if (_handler.TryGetValue(packet.Info.SkillId, out action))
            action.Invoke(cc);

    }
}

