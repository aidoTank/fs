using System;
using ProtoBuf;

namespace Roma
{ 
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
        // 后面改成发送服务器时间，给服务器校验
        SetByte<int>(0, ref ls);
    }

    public override void OnRecv()
    {
        //conn.lastTickTime = Sys.GetTimeStamp();
    }
}
}
