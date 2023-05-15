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
		_handler.Add(100,cool100);
		_handler.Add(101,cool101);
		_handler.Add(102,cool102);
		_handler.Add(103,cool103);
		_handler.Add(104,cool104);
		_handler.Add(110,cool110);
		_handler.Add(200,cool200);
		_handler.Add(201,cool201);
		_handler.Add(202,cool202);
		_handler.Add(203,cool203);
		_handler.Add(204,cool204);
		_handler.Add(205,cool205);
		_handler.Add(300,cool300);
		_handler.Add(301,cool301);
		_handler.Add(302,cool302);
		_handler.Add(303,cool303);
		_handler.Add(304,cool304);

    }

	short cool100= 0;
	short cool101= 0;
	short cool102= 0;
	short cool103= 0;
	short cool104= 0;
	short cool110= 0;
	short cool200= 0;
	short cool201= 0;
	short cool202= 0;
	short cool203= 0;
	short cool204= 0;
	short cool205= 0;
	short cool300= 0;
	short cool301= 0;
	short cool302= 0;
	short cool303= 0;
	short cool304= 0;


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

