using System;
using ProtoBuf;
using Roma;
using UnityEngine;

public class FspMsgPlayerData : NetMessage
{
    public FspMsgPlayerData()
        : base(eNetMessageID.FspMsgPlayerData)
    {
        bFspMsg = true;
    }

    public static NetMessage CreateMessage()
    {
        return new FspMsgPlayerData();
    }

    public override void ToByte(ref LusuoStream ls)
    {
        eno = 0;
        //SetByte<CG_CreateRoom>(m_joinRoom, ref ls);
    }

    public override void OnRecv()
    {
        if (eno == 0)
        {
            Debug.Log("接受玩家信息，开始游戏");
            GameManager.Inst.Start();
        }
    }

    public CG_CreateRoom m_joinRoom = new CG_CreateRoom();
}

