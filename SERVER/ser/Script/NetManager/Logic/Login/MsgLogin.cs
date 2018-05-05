using System;
using ProtoBuf;



public class MsgLogin : NetMessage
{
    public MsgLogin()
        : base(eNetMessageID.MsgLogin)
    {

    }
    public static NetMessage CreateMessage()
    {
        return new MsgLogin();
    }

    public override void ToByte(ref LusuoStream ls)
    {
        SetByte<Roma.GC_PlayerPublicData>(playerData, ref ls);
    }
    
    public override void OnRecv(ref Conn conn)
    {
        CG_Login login = GetData<CG_Login>(structBytes);
        string userName = login.name;
        string pw = login.passWord;

        // 检测账号密码是否正确
        if (!DBPlayer.Inst.CheckPassWord(userName, pw))
        {
            // 不通过，那就先注册
            bool bSucc = DBPlayer.Inst.Register(userName, pw);
            if (!bSucc)   // 注册不成功，账号重复等
            {
                //eno = -1;
                //conn.Send(this);

                eno = 0;
                Console.WriteLine("返回创建角色:" + eno + " uid:" + userName);
                conn.Send(this);
                return;
            }
        }
        TablePlayer player = DBPlayer.Inst.GetPlayer(userName);
        if(player == null)
        {
            // 返回选角色
            eno = 0;
            Console.WriteLine("返回创建角色:" + eno + " uid:"+ userName);
            conn.Send(this);
        }
        else
        {
            // 返回角色信息，进入游戏
            eno = 1;
            Console.WriteLine("返回角色信息:" + eno + " uid:" + userName);
            playerData = CreatePlayer(ref conn, userName, ref player);
            conn.Send(this);

            Lobby map = LobbyManager.Inst.GetMap(player.MapId);
            map.AddPlayer(conn.player);
        }
    }

    /// <summary>
    /// 将表数据组装为逻辑数据
    /// </summary>
    public static Roma.GC_PlayerPublicData CreatePlayer(ref Conn conn, string userName, ref TablePlayer player)
    {
        Roma.GC_PlayerPublicData playerData = new Roma.GC_PlayerPublicData();
        if (player == null)
            return playerData;
        long uid = 0;
        long.TryParse(userName, out uid);
        conn.player = new Player(uid, conn);

        playerData.userName = int.Parse(player.UserName);
        playerData.name = player.Name;
        playerData.resId = player.ResId;
        playerData.occ = player.Occ;
        playerData.mapId = player.MapId;
        playerData.x = player.X;
        playerData.y = player.Y;
        playerData.dir = player.Dir;

        conn.player.data = player;
        conn.player.publicData = playerData;

        return playerData;
    }

    private Roma.GC_PlayerPublicData playerData = new Roma.GC_PlayerPublicData();
}

