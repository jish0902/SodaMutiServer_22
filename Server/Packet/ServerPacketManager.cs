using System;
using System.Collections.Generic;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;

internal class PacketManager
{
    private readonly Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new();

    private readonly Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new();

    private PacketManager()
    {
        Register();
    }

    public Action<PacketSession, IMessage, ushort> CustomHandler { get; set; }

    public void Register()
    {
        _onRecv.Add((ushort)MsgId.CMove, MakePacket<C_Move>);
        _handler.Add((ushort)MsgId.CMove, PacketHandler.C_MoveHandler);
        _onRecv.Add((ushort)MsgId.CEnterGame, MakePacket<C_EnterGame>);
        _handler.Add((ushort)MsgId.CEnterGame, PacketHandler.C_EnterGameHandler);
        _onRecv.Add((ushort)MsgId.CLobbyInfo, MakePacket<C_LobbyInfo>);
        _handler.Add((ushort)MsgId.CLobbyInfo, PacketHandler.C_LobbyInfoHandler);
        _onRecv.Add((ushort)MsgId.CSkill, MakePacket<C_Skill>);
        _handler.Add((ushort)MsgId.CSkill, PacketHandler.C_SkillHandler);
        _onRecv.Add((ushort)MsgId.CHit, MakePacket<C_Hit>);
        _handler.Add((ushort)MsgId.CHit, PacketHandler.C_HitHandler);
        _onRecv.Add((ushort)MsgId.CRewardInfo, MakePacket<C_RewardInfo>);
        _handler.Add((ushort)MsgId.CRewardInfo, PacketHandler.C_RewardInfoHandler);
        _onRecv.Add((ushort)MsgId.CPregameInfo, MakePacket<C_PregameInfo>);
        _handler.Add((ushort)MsgId.CPregameInfo, PacketHandler.C_PregameInfoHandler);
    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {
        ushort count = 0;

        var size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        var id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;
        Action<PacketSession, ArraySegment<byte>, ushort> action = null;
        if (_onRecv.TryGetValue(id, out action))
            action.Invoke(session, buffer, id);
    }

    private void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
    {
        var pkt = new T();
        pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);

        if (CustomHandler != null)
        {
            CustomHandler.Invoke(session, pkt, id);
        }
        else
        {
            Action<PacketSession, IMessage> action = null;
            if (_handler.TryGetValue(id, out action))
                action.Invoke(session, pkt);
        }
    }

    public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
    {
        Action<PacketSession, IMessage> action = null;
        if (_handler.TryGetValue(id, out action))
            return action;
        return null;
    }

    #region Singleton

    public static PacketManager Instance { get; } = new();

    #endregion
}