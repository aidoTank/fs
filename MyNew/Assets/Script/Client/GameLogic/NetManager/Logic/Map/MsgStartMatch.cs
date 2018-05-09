using System;
using ProtoBuf;
using Roma;
using UnityEngine;

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
        SetByte<int>(m_sendMatchType, ref ls);
    }
    
    public override void OnRecv()
    {
        // 接收匹配类型，开始匹配，如果有异常，直接返回
        if (eno == 0)
        {
            GC_MatchResult data = GetData<GC_MatchResult>(structBytes);
            Debug.Log("data ip:" + data.serverIp);
            //GC_MatchResult m_matchResult = new GC_MatchResult();
        }
    }

    public int m_sendMatchType;
    
}

