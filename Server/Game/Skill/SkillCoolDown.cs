using System;
using System.Collections.Generic;
using System.Text;


public class SkillCoolDown
{
    Dictionary<int, short> _handler = new Dictionary<int, short>();

    public SkillCoolDown()
    {
        Register();
    }

    public void Register()
    {
		_handler.Add(10,cool10);
		_handler.Add(11,cool11);
		_handler.Add(100,cool100);
		_handler.Add(500,cool500);
		_handler.Add(501,cool501);

    }

	short cool10= 0;
	short cool11= 0;
	short cool100= 0;
	short cool500= 0;
	short cool501= 0;


    public short GetCoolTime(int id)
    {
        short cool = -1;
        if(_handler.TryGetValue(id, out cool) == true)
        {
            return cool;
        }
        return cool;

    }

    public void SetCoolTime(int id, short time)
    {
        if (_handler.ContainsKey(id) == true)
            _handler[id] = time;
    }


}

