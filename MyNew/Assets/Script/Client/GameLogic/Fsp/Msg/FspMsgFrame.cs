using System;
using ProtoBuf;
using Roma;
using System.Collections.Generic;

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

    public override void OnRecv()
    {
        if (eno == 0)
        {
            //FspFrame frameMsg = GetData<FspFrame>(structBytes);
            //FspServerManager.Inst.GetRoom(3).OnRecvClient(ref conn, frameMsg);
        }
    }

    public List<FspFrame> frameMsg = new List<FspFrame>();
}

