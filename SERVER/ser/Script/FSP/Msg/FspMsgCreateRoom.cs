using System;
using ProtoBuf;
using Roma;

public class FspMsgCreateRoom : NetMessage
{
    public FspMsgCreateRoom()
        : base(eNetMessageID.FspMsgCreateRoom)
    {

    }

    public static NetMessage CreateMessage()
    {
        return new FspMsgCreateRoom();
    }

    public override void ToByte(ref LusuoStream ls)
    {
        eno = 0;
        //SetByte<int>(m_matchResult, ref ls);
    }
    
    public override void OnRecv(ref Conn conn)
    {
        if (eno == 0)
        {
            CG_CreateRoom room = GetData<CG_CreateRoom>(structBytes);
            Console.WriteLine("接受创建房间信息：" + room.userName + " 房间：" + room.roomId);

            FspServerManager.Inst.CreateRoom(room.roomId);

          //  FspServerManager
              //  LobbyManager.Inst.GetLobby(3).OnJoinMatch(conn.player);
            
        }
    }

    public GC_MatchResult m_matchResult = new GC_MatchResult();
}

