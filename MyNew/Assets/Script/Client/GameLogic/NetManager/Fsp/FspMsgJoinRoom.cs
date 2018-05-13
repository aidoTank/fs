using System;
using ProtoBuf;
using Roma;

public class FspMsgJoinRoom : NetMessage
{
    public FspMsgJoinRoom()
        : base(eNetMessageID.FspMsgJoinRoom)
    {
        bFspMsg = true;
    }

    public static NetMessage CreateMessage()
    {
        return new FspMsgJoinRoom();
    }

    public override void ToByte(ref LusuoStream ls)
    {
        eno = 0;
        SetByte<int>(m_curPlayerUid, ref ls);
    }

    public override void OnRecv()
    {
     
        if (eno == 0)
        {
        
        }
    }

    public int m_curPlayerUid = new int();
}

