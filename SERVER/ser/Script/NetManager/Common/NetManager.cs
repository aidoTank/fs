using System.Collections;
using System.Collections.Generic;
using System;

public enum eNetMessageID
{
    MsgHeartBeat = 0,
    MsgLogin = 1,
    MsgCreateRole= 2,
    MsgExit = 3,

    MsgMapLoad = 3,
    MsgMapCreatureEnter = 4,
    MsgMapCreatureLeave = 5,

    MsgUseSkill = 10,
}

public delegate NetMessage CreateMessageEvent();
public class NetManager  : Singleton
{
    public static NetManager Inst;
    public NetManager() 
        : base(true)
    {
        RegNetMessage(eNetMessageID.MsgHeartBeat, MsgHeartBeat.CreateMessage);

        RegNetMessage(eNetMessageID.MsgLogin, MsgLogin.CreateMessage);
        RegNetMessage(eNetMessageID.MsgCreateRole, MsgCreateRole.CreateMessage);
        RegNetMessage(eNetMessageID.MsgExit, MsgExit.CreateMessage);


        RegNetMessage(eNetMessageID.MsgMapCreatureEnter, MsgMapCreatureEnter.CreateMessage);
        RegNetMessage(eNetMessageID.MsgMapCreatureLeave, MsgMapCreatureLeave.CreateMessage);

        RegNetMessage(eNetMessageID.MsgUseSkill, MsgUseSkill.CreateMessage);
    }

    public bool RegNetMessage(eNetMessageID id, CreateMessageEvent msg)
    {
        if (m_listMsg.ContainsKey((ushort)id))
        {
            //Debug.LogError("重复注册消息ID" + uID.ToString());
            return false;
        }
        m_listMsg[(ushort)id] = msg;
        return true;
    }

    public NetMessage GetMessage(ushort id)
    {
        CreateMessageEvent msg;
        if(m_listMsg.TryGetValue(id, out msg))
        {
            return msg();
        }
        Console.WriteLine("[警告]收到未注册的消息id:"+id);
        return null;
    }

    public NetMessage GetMessage(eNetMessageID id)
    {
        CreateMessageEvent msg;
        if (m_listMsg.TryGetValue((ushort)id, out msg))
        {
            return msg();
        }
        return null;
    }

    private Dictionary<ushort, CreateMessageEvent> m_listMsg = new Dictionary<ushort, CreateMessageEvent>();
}

