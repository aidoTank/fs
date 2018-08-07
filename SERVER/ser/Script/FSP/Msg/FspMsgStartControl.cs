using System;
using ProtoBuf;
using Roma;

public class FspMsgStartControl : NetMessage
{
    public FspMsgStartControl()
        : base(eNetMessageID.FspMsgStartControl)
    {
        bFspMsg = true;
    }

    public static NetMessage CreateMessage()
    {
        return new FspMsgStartControl();
    }

    public override void ToByte(ref LusuoStream ls)
    {
        eno = 0;
        //SetByte<int[]>(m_enterInfo, ref ls);
    }
    
    public override void OnRecv(ref Conn conn)
    {
        // 接受玩家的加入房间信息，
        if (eno == 0)
        {
            int c = GetData<int>(structBytes);
            conn.player.tempData.bLoaded = true;
        }
    }

    
}

