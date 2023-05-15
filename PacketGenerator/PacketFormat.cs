namespace PacketGenerator;

internal class PacketFormat
{
    // {0} 패킷 등록
    public static string managerFormat =
        @"using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{{
	#region Singleton
	static PacketManager _instance = new PacketManager();
	public static PacketManager Instance {{ get {{ return _instance; }} }}
	#endregion

	PacketManager()
	{{
		Register();
	}}

	Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
	Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();
	public Action<PacketSession,IMessage,ushort> CustomHandler {{ get; set; }}
		
	public void Register()
	{{
		{0}
	}}

	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
	{{
		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;
		Action<PacketSession, ArraySegment<byte>, ushort> action = null;
		if (_onRecv.TryGetValue(id, out action))
			action.Invoke(session, buffer, id);
		
	}}

	void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
	{{
		T pkt = new T();
		pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);
  
		if(CustomHandler != null)
        {{
			CustomHandler.Invoke(session, pkt, id);
		}}
		else
		{{
		Action<PacketSession, IMessage> action = null;
		if (_handler.TryGetValue(id, out action))
			action.Invoke(session, pkt);
		}}
	}}

	public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
	{{
		Action<PacketSession, IMessage> action = null;
		if (_handler.TryGetValue(id, out action))
			return action;
		return null;
	}}
}}";

    // {0} MsgId
    // {1} 패킷 이름
    public static string managerRegisterFormat =
        @"		
		_onRecv.Add((ushort)MsgId.{0}, MakePacket<{1}>);
		_handler.Add((ushort)MsgId.{0}, PacketHandler.{1}Handler);";


    //{0} 패킷 등록
    public static string c_skillHandlerFormat =
        @"
using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class SkillManager
{{
    #region Singleton
    static SkillManager _instance = new SkillManager();
    public static SkillManager Instance {{ get {{ return _instance; }} }}
    #endregion

    public SkillManager()
    {{
        Register();
    }}

    Dictionary<int,Action<CreatureController>> _handler = new Dictionary<int,Action< CreatureController>>();


    public void Register()
    {{
{0}
    }}

    public void UseSkill(CreatureController cc, S_Skill packet)
    {{
        Action<CreatureController> action;
        if (_handler.TryGetValue(packet.Info.SkillId, out action))
            action.Invoke(cc);

    }}
}}

";

    //{0}skillId
    public static string skillManagerRegisterFormat =
        @"		_handler.Add({0}, SkillHandler.Skill{0});";

    //{0} 패킷 등록
    public static string s_skillHandlerFormat =
        @"using Server.Game;
using System;
using System.Collections.Generic;
using System.Text;


public class SkillManager
{{
    #region Singleton
    static SkillManager _instance = new SkillManager();
    public static SkillManager Instance {{ get {{ return _instance; }} }}
    #endregion

    public SkillManager()
    {{
        Register();
    }}
    Dictionary<int, Action<GameObject>> _handler = new Dictionary<int, Action<GameObject>>();

    public void Register()
    {{
{0}
    }}


    public void UseSkill(GameObject obj,int id)
    {{ 
        Action<GameObject> action = null;
        _handler.TryGetValue(id, out action);
        if(action != null)
            action.Invoke(obj);
    }}


}}



";


    //{0}스킬 등록
    //{1}스킬 변수 생성
    public static string skillCoolDownFormat =
        @"using System;
using System.Collections.Generic;
using System.Text;


public class SkillCoolDown
{{
    Dictionary<int, short> _handler = new Dictionary<int, short>();

    public SkillCoolDown()
    {{
        Register();
    }}

    public void Register()
    {{
{0}
    }}

{1}

    public short GetCoolTime(int id)
    {{
        short cool = -1;
        if(_handler.TryGetValue(id, out cool) == true)
        {{
            return cool;
        }}
        return cool;

    }}

    public void SetCoolTime(int id, short time)
    {{
        if (_handler.ContainsKey(id) == true)
            _handler[id] = time;
    }}


}}

";

    //{0} 스킬 아이디
    public static string SkillCoolHandlerReigsterFormat =
        @"		_handler.Add({0},cool{0});";

    //{0} 스킬 아이디
    public static string SkillCoolMemeberReigsterFormat =
        @"	short cool{0}= 0;";
} //class