using System;
using ProtoBuf;

public class MsgMapCreatureEnter : NetMessage
{
    public MsgMapCreatureEnter()
        : base(eNetMessageID.MsgMapCreatureEnter)
    {

    }

    public static NetMessage CreateMessage()
    {
        return new MsgMapCreatureEnter();
    }

    public override void ToByte(ref LusuoStream ls)
    {
        eno = 0;
        SetByte<Roma.GC_PlayerPublicData>(playerData, ref ls);
    }
    
    public override void OnRecv(ref Conn conn)
    {

    }

    public Roma.GC_PlayerPublicData playerData = new Roma.GC_PlayerPublicData();
}

