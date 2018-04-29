using System;
using ProtoBuf;


public class MsgHeartBeat : NetMessage
{
    public MsgHeartBeat()
        : base(eNetMessageID.MsgHeartBeat)
    {

    }
    public static NetMessage CreateMessage()
    {
        return new MsgHeartBeat();
    }

    public override void ToByte(ref LusuoStream ls)
    {
        //SetByte<GC_PlayerPublicData>(playerData, ref ls);
    }
    
    public override void OnRecv(ref Conn conn)
    {
        conn.lastTickTime = Sys.GetTimeStamp();
    }
}

