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
        SetByte<int>(m_roomId, ref ls);
    }
    
    public override void OnRecv(ref Conn conn)
    {
        // 接受玩家的加入房间信息，
        if (eno == 0)
        {
            CG_CreateRoom room = GetData<CG_CreateRoom>(structBytes);
            Console.WriteLine("接受玩家信息：" + room.roomId);

            // 从大厅获取玩家信息，加入到帧服务器房间,此时暂时给conn赋值玩家信息，没有分服
            Player player = LobbyManager.Inst.GetLobby(3).GetPlayer(room.userName);
            FspServerManager.Inst.GetRoom(room.roomId).AddPlayer(player);
            conn.player = player;
        }
    }

    public int m_roomId;
}

