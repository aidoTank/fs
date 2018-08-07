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
        SetByte<int[]>(m_sendData, ref ls);
    }

    public override void OnRecv(ref Conn conn)
    {
        if (eno == 0)
        {
            //Debug.Log("接受玩家信息，开始游戏");
            
        }
    }

    /// 临时为玩家id
    public int[] m_sendData;
}

