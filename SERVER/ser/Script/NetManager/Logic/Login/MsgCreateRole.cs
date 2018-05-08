using System;
using ProtoBuf;

using Roma;

public class MsgCreateRole : NetMessage
{
    public MsgCreateRole()
      : base(eNetMessageID.MsgCreateRole)
    {

    }

    public static NetMessage CreateMessage()
    {
        return new MsgCreateRole();
    }

    public override void ToByte(ref LusuoStream ls)
    {
        SetByte<Roma.GC_PlayerPublicData>(playerData, ref ls);
    }
    
    public override void OnRecv(ref Conn conn)
    {
       
        CG_CreateRole createRole = GetData<CG_CreateRole>(structBytes);
        Console.WriteLine("userName:" + createRole.userName);
        Console.WriteLine("接受创建name:" + createRole.name);
        Console.WriteLine("接受创建occ:" + createRole.occ);

        TablePlayer playerTable = DBPlayer.Inst.CreatePlayer(createRole.userName, createRole.name, createRole.occ);
        playerTable.MapId = 3;
        playerTable.X = 140;
        playerTable.Y = 170;
        if (playerTable != null)
        {
            eno = 0;
            playerData = MsgLogin.CreatePlayer(ref conn, createRole.userName, ref playerTable);
            Console.WriteLine("创建角色成功，返回角色信息");
            // 进入默认场景
            Lobby map = LobbyManager.Inst.GetMap(playerData.mapId);
            map.AddPlayer(conn.player);
        }
        else
        {
            eno = -1;
            Console.WriteLine("创建角色失败");
        }
        conn.Send(this);
    }

    private Roma.GC_PlayerPublicData playerData = new Roma.GC_PlayerPublicData();
}

