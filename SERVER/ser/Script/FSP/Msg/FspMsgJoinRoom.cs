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
        //SetByte<GC_MatchResult>(m_matchResult, ref ls);
    }
    
    public override void OnRecv(ref Conn conn)
    {
        // 接受玩家的加入房间信息，
        if (eno == 0)
        {
            int uid = GetData<int>(structBytes);
            Console.WriteLine("接受玩家信息：" + uid);

            // 把当前玩家加到指定房间
            Player player = LobbyManager.Inst.GetLobby(3).GetPlayer(uid);
            FspServerManager.Inst.GetRoom(1).AddPlayer(player);
        }
    }

    //public GC_PlayerPublicData m_player = new GC_PlayerPublicData();
}

