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
		_handler.Add(10, SkillHandler.Skill10);
		_handler.Add(11, SkillHandler.Skill11);
		_handler.Add(100, SkillHandler.Skill100);
		_handler.Add(500, SkillHandler.Skill500);
		_handler.Add(501, SkillHandler.Skill501);

    }


    public void UseSkill(GameObject obj,int id)
    { 
        Action<GameObject> action = null;
        _handler.TryGetValue(id, out action);
        if(action != null)
            action.Invoke(obj);
    }


}



