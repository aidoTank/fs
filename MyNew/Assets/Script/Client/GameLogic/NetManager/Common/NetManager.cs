using System.Collections;
using System.Collections.Generic;
using Roma;

public enum eNetMessageID
{
    MsgHeartBeat = 0,
    MsgLogin = 1,
    MsgCreateRole = 2,
    MsgExit = 3,

    MsgStartMatch = 4,  // 接收匹配消息，服务器开始匹配，发送匹配结果

    FspMsgCreateRoom = 100,
    FspMsgJoinRoom = 101,
    FspMsgFrame = 200,

    //MsgMapLoad = 300,
    //MsgMapCreatureEnter,
    //MsgMapCreatureLeave,
    //MsgMapCreatureMove,

    MsgUseSkill,
}

public delegate NetMessage CreateMessageEvent();
public class NetManager : Singleton
{
    public static NetManager Inst;

    public NetManager()
        : base(true)
    {
        RegNetMessage(eNetMessageID.MsgHeartBeat, MsgHeartBeat.CreateMessage);

        RegNetMessage(eNetMessageID.MsgLogin, MsgLogin.CreateMessage);
        RegNetMessage(eNetMessageID.MsgCreateRole, MsgCreateRole.CreateMessage);
        RegNetMessage(eNetMessageID.MsgExit, MsgExit.CreateMessage);
        RegNetMessage(eNetMessageID.MsgStartMatch, MsgStartMatch.CreateMessage);

        RegNetMessage(eNetMessageID.FspMsgCreateRoom, FspMsgCreateRoom.CreateMessage);
        RegNetMessage(eNetMessageID.FspMsgJoinRoom, FspMsgJoinRoom.CreateMessage);
        RegNetMessage(eNetMessageID.FspMsgFrame, FspMsgFrame.CreateMessage);
        //RegNetMessage(eNetMessageID.MsgMapCreatureEnter, MsgMapCreatureEnter.CreateMessage);
        //RegNetMessage(eNetMessageID.MsgMapCreatureLeave, MsgMapCreatureLeave.CreateMessage);
        //RegNetMessage(eNetMessageID.MsgMapCreatureMove, MsgMapCreatureMove.CreateMessage);

        //RegNetMessage(eNetMessageID.MsgUseSkill, MsgUseSkill.CreateMessage);
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

