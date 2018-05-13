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

            //GC_MatchResult m_matchResult = new GC_MatchResult();

            Debug.Log("连接帧服务器:" + data.serverPort);
            FspNetRunTime.Inst = new FspNetRunTime();
            FspNetRunTime.Inst.Init();
            FspNetRunTime.Inst.ConServer(() => {
                FspMsgJoinRoom joinRoom = (FspMsgJoinRoom)NetManager.Inst.GetMessage(eNetMessageID.FspMsgJoinRoom);
                joinRoom.m_curPlayerUid = int.Parse(EGame.m_openid);
                FspNetRunTime.Inst.SendMessage(joinRoom);
            });
        }
    }

    public int m_sendMatchType;
    
}

