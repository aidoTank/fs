using System;
using ProtoBuf;



public class MsgExit : NetMessage
{
    public MsgExit()
        : base(eNetMessageID.MsgExit)
    {

    }
    public static NetMessage CreateMessage()
    {
        return new MsgExit();
    }

    public override void ToByte(ref LusuoStream ls)
    {
        //SetByte<Roma.GC_PlayerPublicData>(playerData, ref ls);
    }
    
    public override void OnRecv(ref Conn conn)
    {
        Console.WriteLine("退出游戏：" + conn.player.publicData.userName);
        conn.player.Logout();
    }
}

