using System;
using ProtoBuf;
using Roma;

public class FspMsgFrame : NetMessage
{
    public FspMsgFrame()
        : base(eNetMessageID.FspMsgFrame)
    {
        bFspMsg = true;
    }

    public static NetMessage CreateMessage()
    {
        return new FspMsgFrame();
    }

    public override void ToByte(ref LusuoStream ls)
    {
        eno = 0;
        SetByte<FspFrame>(frameMsg, ref ls);
    }

    public override void OnRecv(ref Conn conn)
    {
        if (eno == 0)
        {
            FspFrame frameMsg = GetData<FspFrame>(structBytes);
            FspServerManager.Inst.GetRoom(conn.player.tempData.m_roomId).OnRecvClient(ref conn, frameMsg);
        }
    }

    public FspFrame frameMsg = new FspFrame();
}

