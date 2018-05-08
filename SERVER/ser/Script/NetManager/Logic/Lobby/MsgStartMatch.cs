using System;
using ProtoBuf;
using Roma;

public class MsgStartMatch : NetMessage
{
    public MsgStartMatch()
        : base(eNetMessageID.MsgStartMatch)
    {

    }

    public static NetMessage CreateMessage()
    {
        return new MsgStartMatch();
    }

    public override void ToByte(ref LusuoStream ls)
    {
        eno = 0;
        SetByte<GC_MatchResult>(m_matchResult, ref ls);
    }
    
    public override void OnRecv(ref Conn conn)
    {
        // 接收匹配类型，开始匹配，如果有异常，直接返回
        if (eno == 0)
        {
            int type = GetData<int>(structBytes);
            if (type == 1)  // 1V1匹配
            {
                // 开始匹配


                m_matchResult.matchType = 1;
                m_matchResult.serverIp = "127.0.0.1";
                m_matchResult.serverPort = 6001;
            }
        }
    }

    public GC_MatchResult m_matchResult = new GC_MatchResult();
}

