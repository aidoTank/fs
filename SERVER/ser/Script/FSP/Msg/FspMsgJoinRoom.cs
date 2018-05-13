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
        return new MsgStartMatch();
    }

    public override void ToByte(ref LusuoStream ls)
    {
        eno = 0;
        //SetByte<GC_MatchResult>(m_matchResult, ref ls);
    }
    
    public override void OnRecv(ref Conn conn)
    {
        // 接受玩家的加入房间信息，
        if (eno == 0)
        {
            GC_PlayerPublicData playerData = GetData<GC_PlayerPublicData>(structBytes);
            Console.WriteLine("接受玩家信息：" + conn.player.publicData.userName);

            // 把当前玩家加到指定房间
            FspServerManager.Inst.GetRoom(1).AddPlayer(ref conn, playerData);
        }
    }

    //public GC_PlayerPublicData m_player = new GC_PlayerPublicData();
}

