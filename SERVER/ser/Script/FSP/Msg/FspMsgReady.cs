using System;
using ProtoBuf;
using Roma;
using UnityEngine;

public class FspMsgReady : NetMessage
{
    public FspMsgReady()
        : base(eNetMessageID.FspMsgReady)
    {
        //bFspMsg = true;
    }

    public static NetMessage CreateMessage()
    {
        return new FspMsgReady();
    }

    public override void ToByte(ref LusuoStream ls)
    {
        eno = 0;
        //SetByte<CG_CreateRoom>(m_joinRoom, ref ls);
    }

    public override void OnRecv(ref Conn conn)
    {
        if (eno == 0)
        {
            conn.player.tempData.bReady = true;
        }
    }
}

