using Server.Game;
using System;
using System.Collections.Generic;
using System.Text;


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
    Dictionary<int, Action<GameObject>> _handler = new Dictionary<int, Action<GameObject>>();

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


    public void UseSkill(GameObject obj,int id)
    { 
        Action<GameObject> action = null;
        _handler.TryGetValue(id, out action);
        if(action != null)
            action.Invoke(obj);
    }


}



