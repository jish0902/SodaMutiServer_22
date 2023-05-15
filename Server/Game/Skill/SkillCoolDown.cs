using System.Collections.Generic;

public class SkillCoolDown
{
    private readonly Dictionary<int, short> _handler = new();

    private readonly short cool100 = 0;
    private readonly short cool101 = 0;
    private readonly short cool102 = 0;
    private readonly short cool103 = 0;
    private readonly short cool104 = 0;
    private readonly short cool110 = 0;
    private readonly short cool200 = 0;
    private readonly short cool201 = 0;
    private readonly short cool202 = 0;
    private readonly short cool203 = 0;
    private readonly short cool204 = 0;
    private readonly short cool205 = 0;
    private readonly short cool300 = 0;
    private readonly short cool301 = 0;
    private readonly short cool302 = 0;
    private readonly short cool303 = 0;
    private readonly short cool304 = 0;

    public SkillCoolDown()
    {
        Register();
    }

    public void Register()
    {
        _handler.Add(100, cool100);
        _handler.Add(101, cool101);
        _handler.Add(102, cool102);
        _handler.Add(103, cool103);
        _handler.Add(104, cool104);
        _handler.Add(110, cool110);
        _handler.Add(200, cool200);
        _handler.Add(201, cool201);
        _handler.Add(202, cool202);
        _handler.Add(203, cool203);
        _handler.Add(204, cool204);
        _handler.Add(205, cool205);
        _handler.Add(300, cool300);
        _handler.Add(301, cool301);
        _handler.Add(302, cool302);
        _handler.Add(303, cool303);
        _handler.Add(304, cool304);
    }


    public short GetCoolTime(int id)
    {
        short cool = -1;
        if (_handler.TryGetValue(id, out cool)) return cool;
        return cool;
    }

    public void SetCoolTime(int id, short time)
    {
        if (_handler.ContainsKey(id))
            _handler[id] = time;
    }
}